namespace Stott.Security.Optimizely.Features.Csp.Nonce;

using System;
using System.Linq;
using System.Security.Cryptography;

using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

public class DefaultNonceProvider : INonceProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    private readonly ICspSettingsService _cspSettingsService;

    private readonly ICspPermissionService _cspPermissionService;

    private readonly IPageRouteHelper _pageRouteHelper;

    private readonly string? _nonce;

    private bool? _isNonceEnabled;

    private CspSettings? _cspSettings;

    public DefaultNonceProvider(
        ICspSettingsService cspSettingsService,
        ICspPermissionService cspPermissionService,
        IHttpContextAccessor contextAccessor,
        IPageRouteHelper pageRouteHelper)
    {
        _contextAccessor = contextAccessor;
        _cspSettingsService = cspSettingsService;
        _cspPermissionService = cspPermissionService;
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

        return $"'nonce-{_nonce}'";
    }

    protected virtual bool ShouldGenerateNone()
    {
        try
        {
            if (_isNonceEnabled.HasValue)
            {
                return _isNonceEnabled.Value;
            }

            var cspSettings = _cspSettings ?? _cspSettingsService.Get();
            if (cspSettings is not { IsEnabled: true })
            {
                _isNonceEnabled = false;
                return false;
            }

            var sources = _cspPermissionService.GetAsync().GetAwaiter().GetResult();
            _isNonceEnabled = sources.Any(x => string.Equals(x.Source, CspConstants.Sources.Nonce, StringComparison.OrdinalIgnoreCase));
            if (_isNonceEnabled is not true)
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