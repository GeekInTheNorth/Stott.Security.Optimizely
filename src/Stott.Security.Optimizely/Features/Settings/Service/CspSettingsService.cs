namespace Stott.Security.Optimizely.Features.Settings.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Settings.Repository;

internal sealed class CspSettingsService : ICspSettingsService
{
    private readonly ICspSettingsRepository _repository;

    private readonly ICacheWrapper _cache;

    private const string CacheKey = "stott.security.csp.settings";

    public CspSettingsService(ICspSettingsRepository repository, ICacheWrapper cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public CspSettings Get()
    {
        return this.GetAsync().GetAwaiter().GetResult();
    }

    public async Task<CspSettings> GetAsync()
    {
        var settings = _cache.Get<CspSettings>(CacheKey);
        if (settings is null)
        {
            settings = await _repository.GetAsync();

            _cache.Add(CacheKey, settings);
        }

        return settings;
    }

    public async Task SaveAsync(ICspSettings? cspSettings, string? modifiedBy)
    {
        if (cspSettings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.SaveAsync(cspSettings, modifiedBy);

        _cache.RemoveAll();
    }
}