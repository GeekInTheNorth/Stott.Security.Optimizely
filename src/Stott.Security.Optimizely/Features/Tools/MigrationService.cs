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
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class MigrationService : IMigrationService
{
    private readonly ICspSettingsRepository _cspSettingsRepository;

    private readonly ICspPermissionRepository _cspPermissionRepository;

    private readonly ICspSandboxRepository _cspSandboxRepository;

    private readonly ICorsSettingsRepository _corsSettingsRepository;

    private readonly ISecurityHeaderRepository _securityHeaderRepository;

    private readonly IPermissionPolicyRepository _permissionPolicyRepository;

    private readonly IMigrationRepository _migrationRepository;

    private readonly ICacheWrapper _cacheWrapper;

    public MigrationService(
        ICspSettingsRepository cspSettingsRepository,
        ICspPermissionRepository cspPermissionRepository,
        ICspSandboxRepository cspSandboxRepository,
        ICorsSettingsRepository corsSettingsRepository,
        ISecurityHeaderRepository securityHeaderRepository,
        IPermissionPolicyRepository permissionPolicyRepository,
        IMigrationRepository migrationRepository,
        ICacheWrapper cacheWrapper)
    {
        _cspSettingsRepository = cspSettingsRepository;
        _cspPermissionRepository = cspPermissionRepository;
        _cspSandboxRepository = cspSandboxRepository;
        _corsSettingsRepository = corsSettingsRepository;
        _securityHeaderRepository = securityHeaderRepository;
        _permissionPolicyRepository = permissionPolicyRepository;
        _migrationRepository = migrationRepository;
        _cacheWrapper = cacheWrapper;
    }

    public async Task<SettingsModel> Export()
    {
        var cspSettings = await _cspSettingsRepository.GetAsync();
        var cspSources = await _cspPermissionRepository.GetAsync();
        var cspSandbox = await _cspSandboxRepository.GetAsync();
        var corsSettings = await _corsSettingsRepository.GetAsync();
        var headerSettings = await _securityHeaderRepository.GetAsync();
        var permissionPolicies = await _permissionPolicyRepository.GetAsync();

        return new SettingsModel
        {
            Csp = GetCspModel(cspSettings, cspSources, cspSandbox),
            Cors = corsSettings,
            Headers = SecurityHeaderMapper.ToModel(headerSettings),
            PermissionPolicy = permissionPolicies
        };
    }

    public async Task Import(SettingsModel? settings, string? modifiedBy)
    {
        if (settings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _migrationRepository.SaveAsync(settings, modifiedBy);

        _cacheWrapper.RemoveAll();
    }

    private static CspSettingsModel GetCspModel(CspSettings? settings, IList<CspSource>? sources, SandboxModel? sandbox)
    {
        return new CspSettingsModel
        {
            IsEnabled = settings?.IsEnabled ?? false,
            IsReportOnly = settings?.IsReportOnly ?? false,
            IsAllowListEnabled = settings?.IsAllowListEnabled ?? false,
            AllowListUrl = settings?.AllowListUrl,
            IsUpgradeInsecureRequestsEnabled = settings?.IsUpgradeInsecureRequestsEnabled ?? false,
            IsNonceEnabled = settings?.IsNonceEnabled ?? false,
            IsStrictDynamicEnabled = settings?.IsStrictDynamicEnabled ?? false,
            UseInternalReporting = settings?.UseInternalReporting ?? false,
            UseExternalReporting = settings?.UseExternalReporting ?? false,
            ExternalReportToUrl = settings?.ExternalReportToUrl,
            ExternalReportUriUrl = settings?.ExternalReportUriUrl,
            Sandbox = sandbox ?? new SandboxModel(),
            Sources = sources?.Select(GetCspSourceModel).ToList()
        };
    }

    private static CspSourceModel GetCspSourceModel(CspSource? source)
    {
        return new CspSourceModel
        {
            Source = source?.Source ?? string.Empty,
            Directives = source?.Directives
                               ?.Split(new[] { ',', ' ' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                               .ToList() ?? new List<string>()
        };
    }
}