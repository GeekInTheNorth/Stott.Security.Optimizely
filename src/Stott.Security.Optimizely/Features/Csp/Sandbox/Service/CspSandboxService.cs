namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

internal sealed class CspSandboxService : ICspSandboxService
{
    private readonly ICspSandboxRepository _repository;

    private readonly ICacheWrapper _cacheWrapper;

    private const string CacheKeyPrefix = "stott.security.csp.sandbox";

    public CspSandboxService(
        ICspSandboxRepository repository,
        ICacheWrapper cacheWrapper)
    {
        _repository = repository;
        _cacheWrapper = cacheWrapper;
    }

    public async Task<SandboxModel> GetAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(appId, hostName);
        var settings = _cacheWrapper.Get<SandboxModel>(cacheKey);
        if (settings is null)
        {
            settings = await _repository.GetAsync(appId, hostName);

            _cacheWrapper.Add(cacheKey, settings);
        }

        return settings;
    }

    public async Task<CspSandbox?> GetByContextAsync(string? appId, string? hostName)
    {
        return await _repository.GetByContextAsync(appId, hostName);
    }

    public async Task SaveAsync(SandboxModel model, string? modifiedBy, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.SaveAsync(model, modifiedBy, appId, hostName);

        _cacheWrapper.RemoveAll();
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy)
    {
        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(deletedBy))
        {
            return;
        }

        await _repository.DeleteByContextAsync(appId, hostName, deletedBy);

        _cacheWrapper.RemoveAll();
    }

    private static string GetCacheKey(string? appId, string? hostName)
    {
        return $"{CacheKeyPrefix}.{appId ?? "global"}.{hostName ?? "all"}";
    }
}
