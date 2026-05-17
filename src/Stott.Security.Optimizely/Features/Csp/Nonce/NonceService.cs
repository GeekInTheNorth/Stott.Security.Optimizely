using System;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;

namespace Stott.Security.Optimizely.Features.Csp.Nonce;

public sealed class NonceService : INonceService
{
    private readonly ICspSettingsRepository _cspSettingsRepository;

    private readonly ICspPermissionRepository _cspPermissionRepository;

    private readonly ICacheWrapper _cache;

    public NonceService(
        ICspSettingsRepository cspSettingsRepository,
        ICspPermissionRepository cspPermissionRepository,
        ICacheWrapper cache)
    {
        _cspSettingsRepository = cspSettingsRepository;
        _cspPermissionRepository = cspPermissionRepository;
        _cache = cache;
    }

    public async Task<NonceSettings> GetNonceSettingsAsync(Guid? siteId, string? hostName)
    {
        var cacheKey = GetCacheKey(siteId, hostName);
        var nonceSettings = _cache.Get<NonceSettings>(cacheKey);
        if (nonceSettings is not null)
        {
            return nonceSettings;
        }

        nonceSettings = await CreateNonceSettingsAsync(siteId, hostName);
        _cache.Add(cacheKey, nonceSettings);

        return nonceSettings;
    }

    private async Task<NonceSettings> CreateNonceSettingsAsync(Guid? siteId, string? hostName)
    {
        var cspSettings = await _cspSettingsRepository.GetAsync(siteId, hostName);
        if (cspSettings is not { IsEnabled: true })
        {
            return new NonceSettings
            {
                IsEnabled = false,
                Directives = null
            };
        }

        var nonceSource = await _cspPermissionRepository.GetBySourceAsync(CspConstants.Sources.Nonce, siteId, hostName);
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
            Directives = nonceSource?.Directives?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        };

        return nonceSettings;
    }

    private static string GetCacheKey(Guid? siteId, string? hostName)
    {
        var sitePart = siteId.HasValue && siteId.Value != Guid.Empty ? siteId.Value.ToString("N") : "global";
        var hostPart = string.IsNullOrWhiteSpace(hostName) ? string.Empty : hostName.ToLowerInvariant();
        return $"{CspConstants.CacheKeys.CspNonce}.{sitePart}.{hostPart}";
    }
}
