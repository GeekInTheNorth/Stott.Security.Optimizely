namespace Stott.Security.Optimizely.Features.Csp.Settings.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Settings;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;

internal sealed class CspSettingsService : ICspSettingsService
{
    private readonly ICspSettingsRepository _repository;

    private readonly ICacheWrapper _cache;

    public CspSettingsService(ICspSettingsRepository repository, ICacheWrapper cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<CspSettings> GetAsync(Guid? siteId, string? hostName)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspSettings, siteId, hostName);
        var settings = _cache.Get<CspSettings>(cacheKey);
        if (settings is null)
        {
            settings = await _repository.GetAsync(siteId, hostName);

            _cache.Add(cacheKey, settings);
        }

        return settings;
    }

    public async Task SaveAsync(ICspSettings? cspSettings, string? modifiedBy, Guid? siteId, string? hostName)
    {
        if (cspSettings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.SaveAsync(cspSettings, siteId, hostName, modifiedBy);

        _cache.RemoveAll();
    }

    public async Task DeleteByContextAsync(Guid? siteId, string? hostName, string? deletedBy)
    {
        if (!siteId.HasValue || siteId.Value == Guid.Empty || string.IsNullOrWhiteSpace(deletedBy))
        {
            return;
        }

        await _repository.DeleteByContextAsync(siteId, hostName, deletedBy);

        _cache.RemoveAll();
    }

    public async Task<bool> ExistsForContextAsync(Guid? siteId, string? hostName)
    {
        var hasSiteId = siteId.HasValue && siteId.Value != Guid.Empty;
        var hasHostName = !string.IsNullOrWhiteSpace(hostName);
        if (!hasSiteId && !hasHostName)
        {
            // The Global scope always exists.
            return false;
        }

        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspInheritedSettings, siteId, hostName);
        var ctxState = _cache.Get<ContextStateModel>(cacheKey);
        if (ctxState is null)
        {
            var actualSettings = await _repository.GetByContextAsync(siteId, hostName);
            ctxState = new ContextStateModel
            {
                Exists = actualSettings is not null
            };

            _cache.Add(cacheKey, ctxState);
        }

        return ctxState.Exists;
    }

    private static string GetCacheKey(string prefix, Guid? siteId, string? hostName)
    {
        var sitePart = siteId.HasValue && siteId.Value != Guid.Empty ? siteId.Value.ToString("N") : "global";
        var hostPart = string.IsNullOrWhiteSpace(hostName) ? string.Empty : hostName.ToLowerInvariant();
        return $"{prefix}.{sitePart}.{hostPart}";
    }
}
