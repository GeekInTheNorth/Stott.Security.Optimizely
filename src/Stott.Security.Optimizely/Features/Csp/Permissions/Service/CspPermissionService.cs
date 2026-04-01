namespace Stott.Security.Optimizely.Features.Csp.Permissions.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

internal sealed class CspPermissionService(
    ICspPermissionRepository repository,
    ICacheWrapper cacheWrapper) : ICspPermissionService
{
    public async Task AppendDirectiveAsync(string? source, string? directive, string? modifiedBy, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(directive) || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await repository.AppendDirectiveAsync(source, directive, modifiedBy, appId, hostName);

        cacheWrapper.RemoveAll();
    }

    public async Task DeleteAsync(Guid id, string? deletedBy)
    {
        if (id.Equals(Guid.Empty) || string.IsNullOrWhiteSpace(deletedBy))
        {
            return;
        }

        await repository.DeleteAsync(id, deletedBy);

        cacheWrapper.RemoveAll();
    }

    public async Task<IList<CspSource>> GetAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspSources, appId, hostName);
        var sources = cacheWrapper.Get<IList<CspSource>>(cacheKey);
        if (sources is not { Count: > 0 })
        {
            sources = await repository.GetAsync(appId, hostName);
            cacheWrapper.Add(cacheKey, sources);
        }

        return sources ?? [];
    }

    public async Task<IList<CspSource>> GetByContextAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspInheritedSources, appId, hostName);
        var sources = cacheWrapper.Get<IList<CspSource>>(cacheKey);
        if (sources is not { Count: > 0 })
        {
            sources = await repository.GetByContextAsync(appId, hostName);
            cacheWrapper.Add(cacheKey, sources);
        }

        return sources ?? [];
    }

    public async Task SaveAsync(Guid id, string? source, List<string>? directives, string? modifiedBy, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(modifiedBy) || directives is not { Count: > 0 })
        {
            return;
        }

        await repository.SaveAsync(id, source, directives, modifiedBy, appId, hostName);

        cacheWrapper.RemoveAll();
    }

    private static string GetCacheKey(string prefix, string? appId, string? hostName)
    {
        return $"{prefix}.{appId}.{hostName}";
    }
}
