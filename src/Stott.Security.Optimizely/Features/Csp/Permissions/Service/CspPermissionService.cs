namespace Stott.Security.Optimizely.Features.Csp.Permissions.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

internal sealed class CspPermissionService : ICspPermissionService
{
    private readonly ICspPermissionRepository _repository;

    private readonly ICacheWrapper _cache;

    public CspPermissionService(
        ICspPermissionRepository repository,
        ICacheWrapper cache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task AppendDirectiveAsync(string? source, string? directive, string? modifiedBy, Guid? siteId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(directive) || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.AppendDirectiveAsync(source, directive, modifiedBy, siteId, hostName);

        _cache.RemoveAll();
    }

    public async Task DeleteAsync(Guid id, string? deletedBy)
    {
        if (id.Equals(Guid.Empty) || string.IsNullOrWhiteSpace(deletedBy))
        {
            return;
        }

        await _repository.DeleteAsync(id, deletedBy);

        _cache.RemoveAll();
    }

    public async Task<IList<CspSource>> GetAsync(Guid? siteId, string? hostName)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspSources, siteId, hostName);
        var sources = _cache.Get<IList<CspSource>>(cacheKey);
        if (sources is not { Count: > 0 })
        {
            sources = await _repository.GetAsync(siteId, hostName);
            _cache.Add(cacheKey, sources);
        }

        return sources ?? new List<CspSource>(0);
    }

    public async Task<IList<CspSource>> GetAllAsync()
    {
        var sources = _cache.Get<IList<CspSource>>(CspConstants.CacheKeys.CspAllSources);
        if (sources is not { Count: > 0 })
        {
            sources = await _repository.GetAllAsync();
            _cache.Add(CspConstants.CacheKeys.CspAllSources, sources);
        }

        return sources ?? new List<CspSource>(0);
    }

    public async Task SaveAsync(Guid id, string? source, List<string>? directives, string? modifiedBy, Guid? siteId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(modifiedBy) || directives is not { Count: > 0 })
        {
            return;
        }

        await _repository.SaveAsync(id, source, directives, modifiedBy, siteId, hostName);

        _cache.RemoveAll();
    }

    private static string GetCacheKey(string prefix, Guid? siteId, string? hostName)
    {
        var sitePart = siteId.HasValue && siteId.Value != Guid.Empty ? siteId.Value.ToString("N") : "global";
        var hostPart = string.IsNullOrWhiteSpace(hostName) ? string.Empty : hostName.ToLowerInvariant();
        return $"{prefix}.{sitePart}.{hostPart}";
    }
}
