namespace Stott.Security.Optimizely.Features.Settings.Service;

using System.Threading.Tasks;

using EPiServer.ServiceLocation;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Settings.Repository;

internal sealed class CspSettingsService : ICspSettingsService
{
    private ICspSettingsRepository? _settingsRepository;

    private ICspSettingsRepository SettingsRepository
    {
        get
        {
            _settingsRepository ??= ServiceLocator.Current.GetInstance<ICspSettingsRepository>();

            return _settingsRepository;
        }
    }

    private ICacheWrapper? _cacheWrapper;

    private ICacheWrapper CacheWrapper
    {
        get
        {
            _cacheWrapper ??= ServiceLocator.Current.GetInstance<ICacheWrapper>();

            return _cacheWrapper;
        }
    }

    private const string CacheKey = "stott.security.csp.settings";

    public CspSettings Get()
    {
        return this.GetAsync().GetAwaiter().GetResult();
    }

    public async Task<CspSettings> GetAsync()
    {
        var settings = CacheWrapper.Get<CspSettings>(CacheKey);
        if (settings is null)
        {
            settings = await SettingsRepository.GetAsync();

            CacheWrapper.Add(CacheKey, settings);
        }

        return settings;
    }

    public async Task SaveAsync(ICspSettings? cspSettings, string? modifiedBy)
    {
        if (cspSettings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await SettingsRepository.SaveAsync(cspSettings, modifiedBy);

        CacheWrapper.RemoveAll();
    }
}