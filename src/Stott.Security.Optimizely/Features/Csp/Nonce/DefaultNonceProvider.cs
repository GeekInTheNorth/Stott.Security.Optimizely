namespace Stott.Security.Optimizely.Features.Csp.Nonce;

using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Route;

public sealed class DefaultNonceProvider(INonceService nonceService, ISecurityRouteHelper securityRouteHelper) : INonceProvider
{
    private readonly string? _nonce = GenerateSecureNonce();

    private bool? _isNonceEnabled;

    public string? GetNonce()
    {
        return _nonce;
    }

    public async Task<string?> GetCspValueAsync()
    {
        // Optimizely CMS Editor / Admin inteface does not support Nonce
        // So only apply this on content pages
        var shouldGenerateNonce = await ShouldGenerateNonce();

        return shouldGenerateNonce ? $"'nonce-{_nonce}'" : null;
    }

    private async Task<bool> ShouldGenerateNonce()
    {
        try
        {
            if (_isNonceEnabled.HasValue)
            {
                return _isNonceEnabled.Value;
            }

            var routeData = await securityRouteHelper.GetRouteDataAsync();
            if (routeData.RouteType == SecurityRouteType.NoNonceOrHash)
            {
                _isNonceEnabled = false;
                return false;
            }

            var nonceSettings = await nonceService.GetNonceSettingsAsync(routeData);
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