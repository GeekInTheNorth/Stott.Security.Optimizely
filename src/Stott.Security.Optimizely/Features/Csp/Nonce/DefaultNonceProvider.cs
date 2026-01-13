namespace Stott.Security.Optimizely.Features.Csp.Nonce;

using System;
using System.Security.Cryptography;

using Stott.Security.Optimizely.Features.Route;

public class DefaultNonceProvider : INonceProvider
{
    private readonly INonceService _nonceService;

    private readonly ISecurityRouteHelper _securityRouteHelper;

    private readonly string? _nonce;

    private bool? _isNonceEnabled;

    public DefaultNonceProvider(INonceService nonceService, ISecurityRouteHelper securityRouteHelper)
    {
        _nonceService = nonceService;
        _nonce = GenerateSecureNonce();
        _securityRouteHelper = securityRouteHelper;
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

            var routeData = _securityRouteHelper.GetRouteData();
            if (routeData.RouteType == SecurityRouteType.NoNonceOrHash)
            {
                _isNonceEnabled = false;
                return false;
            }

            var nonceSettings = _nonceService.GetNonceSettingsAsync().GetAwaiter().GetResult();
            if (nonceSettings is not { IsEnabled: true, Directives.Count: > 0 })
            {
                _isNonceEnabled = false;
                return false;
            }

            _isNonceEnabled = true;
            return true;
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