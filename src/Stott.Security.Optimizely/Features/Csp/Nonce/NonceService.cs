using System.Linq;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;

namespace Stott.Security.Optimizely.Features.Csp.Nonce;

public sealed class NonceService : INonceService
{
    private readonly ICspSettingsRepository _cspSettingsRepository;
    
    private readonly ICspPermissionRepository _cspPermissionRepository;

    private readonly ICacheWrapper _cache;

    private const string NonceSettingsCacheKey = "stott.security.nonce.settings";

    public NonceService(
        ICspSettingsRepository cspSettingsRepository,
        ICspPermissionRepository cspPermissionRepository,
        ICacheWrapper cache)
    {
        _cspSettingsRepository = cspSettingsRepository;
        _cspPermissionRepository = cspPermissionRepository;
        _cache = cache;
    }

    public async Task<NonceSettings> GetNonceSettingsAsync()
    {
        var nonceSettings = _cache.Get<NonceSettings>(NonceSettingsCacheKey);
        if (nonceSettings is not null)
        {
            return nonceSettings;
        }

        nonceSettings = await CreateNonceSettingsAsync();
        _cache.Add(NonceSettingsCacheKey, nonceSettings);

        return nonceSettings;
    }

    private async Task<NonceSettings> CreateNonceSettingsAsync()
    {
        var cspSettings = await _cspSettingsRepository.GetAsync();
        if (cspSettings is not { IsEnabled: true })
        {
            return new NonceSettings
            {
                IsEnabled = false,
                Directives = null
            };
        }

        var nonceSource = await _cspPermissionRepository.GetBySourceAsync(CspConstants.Sources.Nonce);
        if (nonceSource is not { Directives.Length: > 0 })
        {
            return new NonceSettings
            {
                IsEnabled = false,
                Directives = null
            };
        }

        var nonceSettings = new NonceSettings
        {
            IsEnabled = cspSettings?.IsNonceEnabled ?? false,
            Directives = nonceSource?.Directives?.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries).ToList() ?? null
        };

        return nonceSettings;
    }
}
