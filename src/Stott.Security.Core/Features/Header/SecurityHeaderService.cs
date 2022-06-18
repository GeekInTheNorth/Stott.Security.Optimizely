namespace Stott.Security.Core.Features.Header;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.Permissions.Repository;
using Stott.Security.Core.Features.SecurityHeaders.Enums;
using Stott.Security.Core.Features.SecurityHeaders.Repository;
using Stott.Security.Core.Features.Settings.Repository;

public class SecurityHeaderService : ISecurityHeaderService
{
    private readonly ICspPermissionRepository _cspPermissionRepository;

    private readonly ICspSettingsRepository _cspSettingsRepository;

    private readonly ISecurityHeaderRepository _securityHeaderRepository;

    private readonly ICspContentBuilder _cspContentBuilder;

    private readonly ICacheWrapper _cacheWrapper;

    public SecurityHeaderService(
        ICspPermissionRepository cspPermissionRepository,
        ICspSettingsRepository cspSettingsRepository,
        ISecurityHeaderRepository securityHeaderRepository,
        ICspContentBuilder cspContentBuilder,
        ICacheWrapper cacheWrapper)
    {
        _cspPermissionRepository = cspPermissionRepository;
        _cspSettingsRepository = cspSettingsRepository;
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
            var cspContent = await GetCspContentAsync();

            if (cspSettings.IsReportOnly)
            {
                securityHeaders.Add(CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy, cspContent);
            }
            else
            {
                securityHeaders.Add(CspConstants.HeaderNames.ContentSecurityPolicy, cspContent);
            }
        }

        var securityHeaderSettings = await _securityHeaderRepository.GetAsync();
        if (securityHeaderSettings == null)
        {
            return securityHeaders;
        }

        if (securityHeaderSettings.IsXContentTypeOptionsEnabled)
        {
            securityHeaders.Add(CspConstants.HeaderNames.ContentTypeOptions, CspConstants.HeaderNames.ContentTypeOptionsValue);
        }

        if (securityHeaderSettings.IsXXssProtectionEnabled)
        {
            securityHeaders.Add(CspConstants.HeaderNames.XssProtection, CspConstants.HeaderNames.XssProtectionValue);
        }

        if (securityHeaderSettings.ReferrerPolicy != ReferrerPolicy.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.ReferrerPolicy, GetReferrerPolicyText(securityHeaderSettings.ReferrerPolicy));
        }

        if (securityHeaderSettings.FrameOptions != XFrameOptions.None)
        {
            securityHeaders.Add(CspConstants.HeaderNames.FrameOptions, GetXFrameOptionsText(securityHeaderSettings.FrameOptions));
        }

        return securityHeaders;
    }

    private async Task<string> GetCspContentAsync()
    {
        var cspSources = await _cspPermissionRepository.GetAsync() ?? new List<CspSource>(0);
        var cmsReqirements = _cspPermissionRepository.GetCmsRequirements() ?? new List<CspSource>(0);

        var allSources = cspSources.Union(cmsReqirements).ToList();

        return _cspContentBuilder.WithSources(allSources).BuildAsync();
    }

    private static string GetReferrerPolicyText(ReferrerPolicy referrerPolicy)
    {
        return referrerPolicy switch
        {
            ReferrerPolicy.NoReferrer => "no-referrer",
            ReferrerPolicy.NoReferrerWhenDowngrade => "no-referrer-when-downgrade",
            ReferrerPolicy.Origin => "origin",
            ReferrerPolicy.OriginWhenCrossOrigin => "origin-when-cross-origin",
            ReferrerPolicy.SameOrigin => "same-origin",
            ReferrerPolicy.StrictOrigin => "strict-origin",
            ReferrerPolicy.StrictOriginWhenCrossOrigin => "strict-origin-when-cross-origin",
            ReferrerPolicy.UnsafeUrl => "unsafe-url",
            _ => string.Empty
        };
    }

    private static string GetXFrameOptionsText(XFrameOptions xFrameOptions)
    {
        return xFrameOptions switch
        {
            XFrameOptions.SameOrigin => "SAMEORIGIN",
            XFrameOptions.Deny => "DENY",
            _ => string.Empty
        };
    }
}
