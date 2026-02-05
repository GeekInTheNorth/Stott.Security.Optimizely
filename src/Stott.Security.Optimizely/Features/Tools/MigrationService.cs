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

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class MigrationService(
    ICspSettingsRepository cspSettingsRepository,
    ICspPermissionRepository cspPermissionRepository,
    ICspSandboxRepository cspSandboxRepository,
    ICorsSettingsRepository corsSettingsRepository,
    IPermissionPolicyRepository permissionPolicyRepository,
    IMigrationRepository migrationRepository,
    ICacheWrapper cacheWrapper) : IMigrationService
{
    private static readonly char[] separator = { ',', ' ' };

    public async Task<SettingsModel> Export()
    {
        var cspSettings = await cspSettingsRepository.GetAsync();
        var cspSources = await cspPermissionRepository.GetAsync();
        var cspSandbox = await cspSandboxRepository.GetAsync();
        var corsSettings = await corsSettingsRepository.GetAsync();
        var permissionPolicySettings = await permissionPolicyRepository.GetSettingsAsync();
        var permissionPolicies = await permissionPolicyRepository.ListDirectivesAsync();

        return new SettingsModel
        {
            Csp = GetCspModel(cspSettings, cspSources, cspSandbox),
            Cors = corsSettings,
            PermissionPolicy = new PermissionPolicyModel
            {
                IsEnabled = permissionPolicySettings.IsEnabled,
                Directives = permissionPolicies
            }
        };
    }

    public async Task Import(SettingsModel? settings, string? modifiedBy)
    {
        if (settings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await migrationRepository.SaveAsync(settings, modifiedBy);

        cacheWrapper.RemoveAll();
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
            UseInternalReporting = settings?.UseInternalReporting ?? false,
            UseExternalReporting = settings?.UseExternalReporting ?? false,
            ExternalReportToUrl = settings?.ExternalReportToUrl,
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
                               ?.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                               .ToList() ?? []
        };
    }
}