namespace Stott.Security.Optimizely.Features.Settings.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Settings.Repository;

internal sealed class CspSettingsService : ICspSettingsService
{
    private readonly ICspSettingsRepository _settingsRepository;

    private readonly ICacheWrapper _cacheWrapper;

    public CspSettingsService(
        ICspSettingsRepository repository,
        ICacheWrapper cacheWrapper)
    {
        _settingsRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
    }

    public CspSettings Get()
    {
        return _settingsRepository.GetAsync().GetAwaiter().GetResult();
    }

    public async Task<CspSettings> GetAsync()
    {
        return await _settingsRepository.GetAsync();
    }

    public async Task SaveAsync(ICspSettings? cspSettings, string? modifiedBy)
    {
        if (cspSettings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _settingsRepository.SaveAsync(cspSettings, modifiedBy);

        _cacheWrapper.RemoveAll();
    }
}