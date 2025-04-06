﻿namespace Stott.Security.Optimizely.Features.Csp.Permissions.Service;

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

    private const string CacheKey = "stott.security.csp.sources";

    public CspPermissionService(
        ICspPermissionRepository repository,
        ICacheWrapper cacheWrapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
    }

    public async Task AppendDirectiveAsync(string? source, string? directive, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(directive) || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.AppendDirectiveAsync(source, directive, modifiedBy);

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

    public async Task<IList<CspSource>> GetAsync()
    {
        var sources = _cacheWrapper.Get<IList<CspSource>>(CacheKey);
        if (sources is null)
        {
            sources = await _repository.GetAsync();

            _cacheWrapper.Add(CacheKey, sources);
        }

        return sources;
    }

    public async Task SaveAsync(Guid id, string? source, List<string>? directives, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(modifiedBy) || directives is not { Count: > 0 })
        {
            return;
        }

        await _repository.SaveAsync(id, source, directives, modifiedBy);

        _cacheWrapper.RemoveAll();
    }
}