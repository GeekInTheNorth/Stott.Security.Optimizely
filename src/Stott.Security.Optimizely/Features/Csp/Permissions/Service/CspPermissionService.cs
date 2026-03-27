namespace Stott.Security.Optimizely.Features.Csp.Permissions.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

internal sealed class CspPermissionService : ICspPermissionService
{
    private readonly ICspPermissionRepository _repository;

    private readonly ICacheWrapper _cacheWrapper;

    private const string CacheKeyPrefix = "stott.security.csp.sources";

    public CspPermissionService(
        ICspPermissionRepository repository,
        ICacheWrapper cacheWrapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
    }

    public async Task AppendDirectiveAsync(string? source, string? directive, string? modifiedBy, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(directive) || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.AppendDirectiveAsync(source, directive, modifiedBy, appId, hostName);

        _cacheWrapper.RemoveAll();
    }

    public async Task DeleteAsync(Guid id, string? deletedBy)
    {
        if (id.Equals(Guid.Empty) || string.IsNullOrWhiteSpace(deletedBy))
        {
            return;
        }

        await _repository.DeleteAsync(id, deletedBy);

        _cacheWrapper.RemoveAll();
    }

    public async Task<IList<CspSource>> GetAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(appId, hostName);
        var sources = _cacheWrapper.Get<IList<CspSource>>(cacheKey);
        if (sources is not { Count: > 0 })
        {
            sources = await _repository.GetAsync(appId, hostName);
            _cacheWrapper.Add(cacheKey, sources);
        }

        return sources ?? new List<CspSource>();
    }

    public async Task<IList<CspSource>> GetByContextAsync(string? appId, string? hostName)
    {
        return await _repository.GetByContextAsync(appId, hostName);
    }

    public async Task SaveAsync(Guid id, string? source, List<string>? directives, string? modifiedBy, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(modifiedBy) || directives is not { Count: > 0 })
        {
            return;
        }

        await _repository.SaveAsync(id, source, directives, modifiedBy, appId, hostName);

        _cacheWrapper.RemoveAll();
    }

    private static string GetCacheKey(string? appId, string? hostName)
    {
        return $"{CacheKeyPrefix}.{appId ?? "global"}.{hostName ?? "all"}";
    }
}
