namespace Stott.Security.Optimizely.Features.Csp.Settings.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Settings;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;

internal sealed class CspSettingsService : ICspSettingsService
{
    private readonly ICspSettingsRepository _repository;

    private readonly ICacheWrapper _cache;

    private const string CacheKeyPrefix = "stott.security.csp.settings";

    public CspSettingsService(ICspSettingsRepository repository, ICacheWrapper cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public CspSettings Get(string? appId, string? hostName)
    {
        return GetAsync(appId, hostName).GetAwaiter().GetResult();
    }

    public async Task<CspSettings> GetAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(appId, hostName);
        var settings = _cache.Get<CspSettings>(cacheKey);
        if (settings is null)
        {
            settings = await _repository.GetAsync(appId, hostName);

            _cache.Add(cacheKey, settings);
        }

        return settings;
    }

    public async Task<CspSettings?> GetByContextAsync(string? appId, string? hostName)
    {
        return await _repository.GetByContextAsync(appId, hostName);
    }

    public async Task SaveAsync(ICspSettings? cspSettings, string? modifiedBy, string? appId, string? hostName)
    {
        if (cspSettings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.SaveAsync(cspSettings, modifiedBy, appId, hostName);

        _cache.RemoveAll();
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy)
    {
        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(deletedBy))
        {
            return;
        }

        await _repository.DeleteByContextAsync(appId, hostName, deletedBy);

        _cache.RemoveAll();
    }

    private static string GetCacheKey(string? appId, string? hostName)
    {
        return $"{CacheKeyPrefix}.{appId ?? "global"}.{hostName ?? "all"}";
    }
}
