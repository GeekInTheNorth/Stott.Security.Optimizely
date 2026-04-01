using System.Linq;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;
using Stott.Security.Optimizely.Features.Route;

namespace Stott.Security.Optimizely.Features.Csp.Nonce;

public sealed class NonceService(
    ICspSettingsRepository cspSettingsRepository,
    ICspPermissionRepository cspPermissionRepository,
    ICacheWrapper cache) : INonceService
{

    public async Task<NonceSettings> GetNonceSettingsAsync(SecurityRouteData? routeData)
    {
        var cacheKey = GetCacheKey(routeData);
        var nonceSettings = cache.Get<NonceSettings>(cacheKey);
        if (nonceSettings is not null)
        {
            return nonceSettings;
        }

        nonceSettings = await CreateNonceSettingsAsync(routeData);
        cache.Add(cacheKey, nonceSettings);

        return nonceSettings;
    }

    private async Task<NonceSettings> CreateNonceSettingsAsync(SecurityRouteData? routeData)
    {
        var cspSettings = await cspSettingsRepository.GetAsync(routeData?.AppId, routeData?.HostName);
        if (cspSettings is not { IsEnabled: true })
        {
            return new NonceSettings
            {
                IsEnabled = false,
                Directives = null
            };
        }

        var nonceSource = await cspPermissionRepository.GetBySourceAsync(CspConstants.Sources.Nonce, routeData?.AppId, routeData?.HostName);
        if (nonceSource is not { Directives.Length: > 0 })
        {
            return new NonceSettings
            {
                IsEnabled = false,
                Directives = null
            };
        }

        var nonceSettings = new NonceSettings
        {
            IsEnabled = true,
            Directives = nonceSource?.Directives?.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries).ToList()
        };

        return nonceSettings;
    }

    private static string GetCacheKey(SecurityRouteData? routeData)
    {
        return $"{CspConstants.CacheKeys.CspNonce}.{routeData?.AppId}.{routeData?.HostName}";
    }
}
