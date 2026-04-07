namespace Stott.Security.Optimizely.Features.Csp.Settings.Service;

using System.Threading.Tasks;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Settings;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;

/// <inheritdoc cref="ICspSettingsService"/>
internal sealed class CspSettingsService(ICspSettingsRepository repository, ICacheWrapper cache) : ICspSettingsService
{
    public async Task<CspSettings> GetAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspSettings, appId, hostName);
        var settings = cache.Get<CspSettings>(cacheKey);
        if (settings is null)
        {
            settings = await repository.GetAsync(appId, hostName);

            cache.Add(cacheKey, settings);
        }

        return settings;
    }

    public async Task SaveAsync(ICspSettings? cspSettings, string? modifiedBy, string? appId, string? hostName)
    {
        if (cspSettings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await repository.SaveAsync(cspSettings, modifiedBy, appId, hostName);

        cache.RemoveAll();
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy)
    {
        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(deletedBy))
        {
            return;
        }

        await repository.DeleteByContextAsync(appId, hostName, deletedBy);

        cache.RemoveAll();
    }

    public async Task<bool> ExistsForContextAsync(string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(appId) && string.IsNullOrWhiteSpace(hostName))
        {
            return false;
        }

        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspInheritedSettings, appId, hostName);
        var ctxState = cache.Get<ContextStateModel>(cacheKey);
        if (ctxState is null)
        {
            var actualSettings = await repository.GetByContextAsync(appId, hostName);
            ctxState = new ContextStateModel
            {
                Exists = actualSettings is not null
            };

            cache.Add(cacheKey, ctxState);
        }

        return ctxState.Exists;
    }

    private static string GetCacheKey(string prefix, string? appId, string? hostName)
    {
        return $"{prefix}.{appId}.{hostName}";
    }
}
