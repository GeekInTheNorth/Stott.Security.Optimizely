namespace Stott.Security.Optimizely.Features.Cors.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors.Repository;

internal sealed class CorsSettingsService : ICorsSettingsService
{
    private readonly ICacheWrapper _cache;

    private readonly ICorsSettingsRepository _repository;

    private const string CacheKey = "stott.security.cors.data";

    public CorsSettingsService(ICacheWrapper cache, ICorsSettingsRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<CorsConfiguration> GetAsync()
    {
        var configuration = _cache.Get<CorsConfiguration>(CacheKey);
        if (configuration == null)
        {
            configuration = await _repository.GetAsync();
            _cache.Add(CacheKey, configuration);
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

        _cache.RemoveAll();

        await _repository.SaveAsync(model, modifiedBy);
    }
}