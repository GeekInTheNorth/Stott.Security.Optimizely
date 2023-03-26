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

    public async Task<CspSettings> GetAsync()
    {
        return await _settingsRepository.GetAsync();
    }

    public async Task SaveAsync(CspSettingsModel? cspSettings, string? modifiedBy)
    {
        if (cspSettings is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _settingsRepository.SaveAsync(
            cspSettings.IsEnabled, 
            cspSettings.IsReportOnly, 
            cspSettings.IsWhitelistEnabled, 
            cspSettings.WhitelistAddress ?? string.Empty,
            modifiedBy);

        _cacheWrapper.RemoveAll();
    }
}