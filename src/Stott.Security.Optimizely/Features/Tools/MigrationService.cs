using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;
using Stott.Security.Optimizely.Features.Tools.Models;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class MigrationService : IMigrationService
{
    private readonly ICspSettingsRepository _cspSettingsRepository;

    private readonly ICspPermissionRepository _cspPermissionRepository;
    
    private readonly ICspSandboxRepository _cspSandboxRepository;
    
    private readonly ICorsSettingsRepository _corsSettingsRepository;
    
    private readonly IPermissionPolicyRepository _permissionPolicyRepository;
    
    private readonly ICustomHeaderRepository _customHeaderRepository;
    
    private readonly IMigrationRepository _migrationRepository;
    
    private readonly ICacheWrapper _cacheWrapper;

    private static readonly char[] separator = { ',', ' ' };

    public MigrationService(
        ICspSettingsRepository cspSettingsRepository, 
        ICspPermissionRepository cspPermissionRepository, 
        ICspSandboxRepository cspSandboxRepository, 
        ICorsSettingsRepository corsSettingsRepository, 
        IPermissionPolicyRepository permissionPolicyRepository, 
        ICustomHeaderRepository customHeaderRepository, 
        IMigrationRepository migrationRepository, 
        ICacheWrapper cacheWrapper)
    {
        _cspSettingsRepository = cspSettingsRepository;
        _cspPermissionRepository = cspPermissionRepository;
        _cspSandboxRepository = cspSandboxRepository;
        _corsSettingsRepository = corsSettingsRepository;
        _permissionPolicyRepository = permissionPolicyRepository;
        _customHeaderRepository = customHeaderRepository;
        _migrationRepository = migrationRepository;
        _cacheWrapper = cacheWrapper;
    }

    public async Task<SettingsModel> Export(Guid? siteId = null, string? hostName = null)
    {
        var cspSettings = await _cspSettingsRepository.GetAsync(siteId, hostName);
        var cspSources = await _cspPermissionRepository.GetAsync(siteId, hostName);
        var cspSandbox = await _cspSandboxRepository.GetAsync(siteId, hostName);
        var corsSettings = await _corsSettingsRepository.GetAsync();
        var permissionPolicySettings = await _permissionPolicyRepository.GetSettingsAsync(siteId, hostName);
        var permissionPolicies = await _permissionPolicyRepository.ListDirectivesAsync(siteId, hostName);
        var customHeaders = await _customHeaderRepository.GetAllAsync(siteId, hostName);

        return new SettingsModel
        {
            Csp = GetCspModel(cspSettings, cspSources, cspSandbox),
            Cors = corsSettings,
            PermissionPolicy = GetPermissionPolicyModel(permissionPolicySettings, permissionPolicies),
            CustomHeaders = customHeaders.Select(GetCustomHeaderModel).ToList()
        };
    }

    public async Task Import(SettingsModel? settings, string? modifiedBy, Guid? siteId = null, string? hostName = null)
    {
        if (settings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _migrationRepository.SaveAsync(settings, modifiedBy, siteId, hostName);

        _cacheWrapper.RemoveAll();
    }

    private static PermissionPolicyMigrationModel GetPermissionPolicyModel(PermissionPolicySettingsModel settings, IList<PermissionPolicyDirectiveModel> directives)
    {
        return new PermissionPolicyMigrationModel
        {
            IsEnabled = settings.IsEnabled,
            Directives = directives.Select(d => new PermissionPolicyDirectiveMigrationModel
            {
                Name = d.Name,
                EnabledState = d.EnabledState,
                Sources = d.Sources
            }).ToList()
        };
    }

    private static CspSettingsMigrationModel GetCspModel(CspSettings? settings, IList<CspSource>? sources, SandboxModel? sandbox)
    {
        return MigrationMapper.ConvertToModel(settings, sources, sandbox);
    }

    private static CspSourceMigrationModel GetCspSourceModel(CspSource? source)
    {
        return new CspSourceMigrationModel
        {
            Source = source?.Source ?? string.Empty,
            Directives = source?.Directives
                               ?.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                               .ToList() ?? new List<string>()
        };
    }

    private static CustomHeaderMigrationModel GetCustomHeaderModel(CustomHeader header)
    {
        return new CustomHeaderMigrationModel
        {
            HeaderName = header.HeaderName,
            Behavior = header.Behavior,
            HeaderValue = header.HeaderValue
        };
    }
}