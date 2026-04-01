namespace Stott.Security.Optimizely.Features.Cors.Service;

using System;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors.Repository;

internal sealed class CorsSettingsService(ICacheWrapper cache, ICorsSettingsRepository repository) : ICorsSettingsService
{
    public async Task<CorsConfiguration> GetAsync()
    {
        var configuration = cache.Get<CorsConfiguration>(CspConstants.CacheKeys.CorsSettings);
        if (configuration == null)
        {
            configuration = await repository.GetAsync();
            cache.Add(CspConstants.CacheKeys.CorsSettings, configuration);
        }

        return configuration;
    }

    public async Task SaveAsync(CorsConfiguration? model, string? modifiedBy)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentNullException(nameof(modifiedBy));
        }

        cache.RemoveAll();

        await repository.SaveAsync(model, modifiedBy);
    }
}