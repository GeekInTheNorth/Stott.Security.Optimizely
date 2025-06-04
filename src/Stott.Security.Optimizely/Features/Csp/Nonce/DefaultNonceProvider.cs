namespace Stott.Security.Optimizely.Features.Csp.Nonce;

using System;
using System.Security.Cryptography;

using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

public class DefaultNonceProvider : INonceProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    private readonly IPageRouteHelper _pageRouteHelper;

    private readonly CspSettings _settings;

    private readonly string? _nonce;

    public DefaultNonceProvider(
        ICspSettingsService settingsService,
        IHttpContextAccessor contextAccessor,
        IPageRouteHelper pageRouteHelper)
    {
        _contextAccessor = contextAccessor;
        _settings = settingsService.Get();
        _nonce = GenerateSecureNonce();
        _pageRouteHelper = pageRouteHelper;
    }

    public string? GetNonce()
    {
        // Optimizely CMS Editor / Admin inteface does not support Nonce
        // So only apply this on content pages
        if (!ShouldGenerateNone())
        {
            return null;
        }

        return _nonce;
    }

    public string? GetCspValue()
    {
        // Optimizely CMS Editor / Admin inteface does not support Nonce
        // So only apply this on content pages
        if (!ShouldGenerateNone())
        {
            return null;
        }

        var strictDynamicValue = _settings.IsStrictDynamicEnabled ? CspConstants.StrictDynamic : null;

        return $"'nonce-{_nonce}' {strictDynamicValue}";
    }

    protected virtual bool ShouldGenerateNone()
    {
        try
        {
            if (_settings is not { IsEnabled: true, IsNonceEnabled: true })
            {
                return false;
            }

            var isHeaderListApi = _contextAccessor.HttpContext?.Request?.Path.StartsWithSegments("/stott.security.optimizely/api/compiled-headers") ?? false;

            // .PageLink has a value for Geta Categories while .Page is null
            var isContentPage = _pageRouteHelper.PageLink is { ID: > 0 };

            return isHeaderListApi || isContentPage;
        }
        catch (Exception)
        {
            return false;
        }   
    }

    private static string GenerateSecureNonce()
    {
        const int nonceSize = 32; // 32 bytes = 256 bits
        var nonceBytes = new byte[nonceSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(nonceBytes);
        }

        // Convert to Base64 for use in CSP headers
        return Convert.ToBase64String(nonceBytes);
    }
}