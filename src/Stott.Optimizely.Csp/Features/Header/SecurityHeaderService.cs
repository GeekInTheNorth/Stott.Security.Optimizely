using System.Collections.Generic;
using System.Linq;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Repository;
using Stott.Optimizely.Csp.Features.Settings.Repository;

namespace Stott.Optimizely.Csp.Features.Header
{
    public class SecurityHeaderService : ISecurityHeaderService
    {
        private readonly ICspPermissionRepository _cspPermissionRepository;

        private readonly ICspSettingsRepository _cspSettingsRepository;

        private readonly ISecurityHeaderRepository _securityHeaderRepository;

        private readonly ICspContentBuilder _cspContentBuilder;

        public SecurityHeaderService(
            ICspPermissionRepository cspPermissionRepository,
            ICspSettingsRepository cspSettingsRepository,
            ISecurityHeaderRepository securityHeaderRepository,
            ICspContentBuilder cspContentBuilder)
        {
            _cspPermissionRepository = cspPermissionRepository;
            _cspSettingsRepository = cspSettingsRepository;
            _securityHeaderRepository = securityHeaderRepository;
            _cspContentBuilder = cspContentBuilder;
        }

        public Dictionary<string, string> GetSecurityHeaders()
        {
            var securityHeaders = new Dictionary<string, string>();
            
            var cspSettings = _cspSettingsRepository.Get();
            if (cspSettings?.IsEnabled ?? false)
            {
                securityHeaders.Add(CspConstants.HeaderNames.ContentSecurityPolicy, GetCspContent());
            }

            var securityHeaderSettings = _securityHeaderRepository.Get();
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

        private string GetCspContent()
        {
            var cspSources = _cspPermissionRepository.Get() ?? new List<CspSource>(0);
            var cmsReqirements = _cspPermissionRepository.GetCmsRequirements() ?? new List<CspSource>(0);

            var allSources = cspSources.Union(cmsReqirements).ToList();

            return _cspContentBuilder.WithSources(allSources).Build();
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
}
