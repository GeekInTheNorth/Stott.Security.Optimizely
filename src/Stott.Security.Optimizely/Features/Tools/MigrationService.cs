using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Features.Permissions.Repository;
using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Sandbox.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;
using Stott.Security.Optimizely.Features.Settings.Repository;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class MigrationService : IMigrationService
{
    private readonly ICspSettingsRepository _cspSettingsRepository;

    private readonly ICspPermissionRepository _cspPermissionRepository;

    private readonly ICspSandboxRepository _cspSandboxRepository;

    private readonly ICorsSettingsRepository _corsSettingsRepository;

    private readonly ISecurityHeaderRepository _securityHeaderRepository;

    public MigrationService(
        ICspSettingsRepository cspSettingsRepository, 
        ICspPermissionRepository cspPermissionRepository, 
        ICspSandboxRepository cspSandboxRepository, 
        ICorsSettingsRepository corsSettingsRepository, 
        ISecurityHeaderRepository securityHeaderRepository)
    {
        _cspSettingsRepository = cspSettingsRepository;
        _cspPermissionRepository = cspPermissionRepository;
        _cspSandboxRepository = cspSandboxRepository;
        _corsSettingsRepository = corsSettingsRepository;
        _securityHeaderRepository = securityHeaderRepository;
    }

    public async Task<SettingsModel> Export()
    {
        var settingsTask = await _cspSettingsRepository.GetAsync();
        var sourcesTask = await _cspPermissionRepository.GetAsync();
        var sandboxTask = await _cspSandboxRepository.GetAsync();
        var corsTask = await _corsSettingsRepository.GetAsync();
        var miscHeadersTask = await _securityHeaderRepository.GetAsync();

        return new SettingsModel
        {
            Csp = GetCspModel(settingsTask, sourcesTask, sandboxTask),
            Cors = corsTask,
            Headers = GetSecurityHeaders(miscHeadersTask)
        };
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

    private static SecurityHeadersModel GetSecurityHeaders(SecurityHeaderSettings? settings)
    {
        return new SecurityHeadersModel
        {
            XContentTypeOptions = settings?.XContentTypeOptions.ToString() ?? XContentTypeOptions.None.ToString(),
            XssProtection = settings?.XssProtection.ToString() ?? XssProtection.None.ToString(),
            ReferrerPolicy = settings?.ReferrerPolicy.ToString() ?? ReferrerPolicy.None.ToString(),
            FrameOptions = settings?.FrameOptions.ToString() ?? XFrameOptions.None.ToString(),
            CrossOriginEmbedderPolicy = settings?.CrossOriginEmbedderPolicy.ToString() ?? CrossOriginEmbedderPolicy.None.ToString(),
            CrossOriginOpenerPolicy = settings?.CrossOriginOpenerPolicy.ToString() ?? CrossOriginOpenerPolicy.None.ToString(),
            CrossOriginResourcePolicy = settings?.CrossOriginResourcePolicy.ToString() ?? CrossOriginResourcePolicy.None.ToString(),
            IsStrictTransportSecurityEnabled = settings?.IsStrictTransportSecurityEnabled ?? false,
            IsStrictTransportSecuritySubDomainsEnabled = settings?.IsStrictTransportSecuritySubDomainsEnabled ?? false,
            StrictTransportSecurityMaxAge = settings?.StrictTransportSecurityMaxAge ?? 0,
            ForceHttpRedirect = settings?.ForceHttpRedirect ?? false
        };
    }
}