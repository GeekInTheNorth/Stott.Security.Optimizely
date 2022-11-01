namespace Stott.Security.Optimizely.Features.Settings.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Settings.Repository;

public class CspSettingsService : ICspSettingsService
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

    public async Task SaveAsync(CspSettingsModel cspSettings, string modifiedBy)
    {
        if (cspSettings == null) throw new ArgumentNullException(nameof(cspSettings));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await _settingsRepository.SaveAsync(
            cspSettings.IsEnabled, 
            cspSettings.IsReportOnly, 
            cspSettings.IsWhitelistEnabled, 
            cspSettings.WhitelistAddress,
            modifiedBy);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }
}