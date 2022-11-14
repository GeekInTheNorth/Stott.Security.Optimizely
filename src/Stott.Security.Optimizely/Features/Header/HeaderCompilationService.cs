namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Permissions.Repository;
using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Sandbox.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;
using Stott.Security.Optimizely.Features.Settings.Repository;

public class HeaderCompilationService : IHeaderCompilationService
{
    private readonly ICspPermissionRepository _cspPermissionRepository;

    private readonly ICspSettingsRepository _cspSettingsRepository;

    private readonly ICspSandboxRepository _cspSandboxRepository;

    private readonly ISecurityHeaderRepository _securityHeaderRepository;

    private readonly ICspContentBuilder _cspContentBuilder;

    private readonly ICacheWrapper _cacheWrapper;

    public HeaderCompilationService(
        ICspPermissionRepository cspPermissionRepository,
        ICspSettingsRepository cspSettingsRepository,
        ICspSandboxRepository cspSandboxRepository,
        ISecurityHeaderRepository securityHeaderRepository,
        ICspContentBuilder cspContentBuilder,
        ICacheWrapper cacheWrapper)
    {
        _cspPermissionRepository = cspPermissionRepository;
        _cspSettingsRepository = cspSettingsRepository;
        _cspSandboxRepository = cspSandboxRepository;
        _securityHeaderRepository = securityHeaderRepository;
        _cspContentBuilder = cspContentBuilder;
        _cacheWrapper = cacheWrapper;
    }

    public async Task<Dictionary<string, string>> GetSecurityHeadersAsync()
    {
        var headers = _cacheWrapper.Get<Dictionary<string, string>>(CspConstants.CacheKeys.CompiledCsp);
        if (headers == null)
        {
            headers = await CompileSecurityHeadersAsync();

            _cacheWrapper.Add(CspConstants.CacheKeys.CompiledCsp, headers);
        }

        return headers;
    }

    private async Task<Dictionary<string, string>> CompileSecurityHeadersAsync()
    {
        var securityHeaders = new Dictionary<string, string>();

        var cspSettings = await _cspSettingsRepository.GetAsync();
        if (cspSettings?.IsEnabled ?? false)
        {
            var cspContent = await GetCspContentAsync(cspSettings);

            if (cspSettings.IsReportOnly)
            {
                securityHeaders.Add(CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy, cspContent);
            }
            else
            {
                securityHeaders.Add(CspConstants.HeaderNames.ContentSecurityPolicy, cspContent);
            }
        }

        var headerSettings = await _securityHeaderRepository.GetAsync();
        if (headerSettings == null)
        {
            return securityHeaders;
        }

        if (headerSettings.XContentTypeOptions != XContentTypeOptions.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.ContentTypeOptions, headerSettings.XContentTypeOptions.GetSecurityHeaderValue());
        }

        if (headerSettings.XssProtection != XssProtection.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.XssProtection, headerSettings.XssProtection.GetSecurityHeaderValue());
        }

        if (headerSettings.ReferrerPolicy != ReferrerPolicy.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.ReferrerPolicy, headerSettings.ReferrerPolicy.GetSecurityHeaderValue());
        }

        if (headerSettings.FrameOptions != XFrameOptions.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.FrameOptions, headerSettings.FrameOptions.GetSecurityHeaderValue());
        }

        if (headerSettings.CrossOriginEmbedderPolicy != CrossOriginEmbedderPolicy.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.CrossOriginEmbedderPolicy, headerSettings.CrossOriginEmbedderPolicy.GetSecurityHeaderValue());
        }

        if (headerSettings.CrossOriginOpenerPolicy != CrossOriginOpenerPolicy.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.CrossOriginOpenerPolicy, headerSettings.CrossOriginOpenerPolicy.GetSecurityHeaderValue());
        }

        if (headerSettings.CrossOriginResourcePolicy != CrossOriginResourcePolicy.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.CrossOriginResourcePolicy, headerSettings.CrossOriginResourcePolicy.GetSecurityHeaderValue());
        }

        if (headerSettings.IsStrictTransportSecurityEnabled)
        {
            securityHeaders.Add(CspConstants.HeaderNames.StrictTransportSecurity, GetStrictTransportSecurityValue(headerSettings));
        }

        return securityHeaders;
    }

    private async Task<string> GetCspContentAsync(CspSettings cspSettings)
    {
        var cspSandbox = await _cspSandboxRepository.GetAsync() ?? new SandboxModel();
        var cspSources = await _cspPermissionRepository.GetAsync() ?? new List<CspSource>(0);
        var cmsReqirements = _cspPermissionRepository.GetCmsRequirements() ?? new List<CspSource>(0);

        var allSources = cspSources.Union(cmsReqirements).ToList();

        return _cspContentBuilder.WithSources(allSources)
                                 .WithSettings(cspSettings)
                                 .WithSandbox(cspSandbox)
                                 .BuildAsync();
    }

    private static string GetStrictTransportSecurityValue(SecurityHeaderSettings headerSettings)
    {
        return headerSettings.IsStrictTransportSecuritySubDomainsEnabled ?
            $"max-age={headerSettings.StrictTransportSecurityMaxAge}; includeSubDomains" :
            $"max-age={headerSettings.StrictTransportSecurityMaxAge}";
    }
}
