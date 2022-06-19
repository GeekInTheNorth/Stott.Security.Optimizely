namespace Stott.Security.Core.Features.Settings.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.Settings.Repository;

public class CspSettingsService : ICspSettingsService
{
    private readonly ICspSettingsRepository _repository;

    private readonly ICacheWrapper _cacheWrapper;

    public CspSettingsService(
        ICspSettingsRepository repository,
        ICacheWrapper cacheWrapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
    }

    public async Task<CspSettings> GetAsync()
    {
        return await _repository.GetAsync();
    }

    public async Task SaveAsync(bool isEnabled, bool isReportOnly)
    {
        await _repository.SaveAsync(isEnabled, isReportOnly);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }
}
