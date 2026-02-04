namespace Stott.Security.Optimizely.Features.CustomHeaders.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;
using Stott.Security.Optimizely.Features.Header;

/// <summary>
/// Service implementation for custom header business logic.
/// </summary>
internal sealed class CustomHeaderService(ICustomHeaderRepository repository, ICacheWrapper cache) : ICustomHeaderService
{
    private const string CacheKey = "stott.security.customheaders";

    public async Task<IList<CustomHeaderModel>> GetAllAsync()
    {
        var cachedHeaders = cache.Get<List<CustomHeaderModel>>(CacheKey);
        if (cachedHeaders is not null)
        {
            return cachedHeaders;
        }

        var headers = await repository.GetAllAsync();
        var models = headers.Select(CustomHeaderMapper.ToModel).ToList();

        cache.Add(CacheKey, models);

        return models;
    }

    public async Task<IList<HeaderDto>> GetCompiledHeaders()
    {
        var headers = await GetAllAsync();
        return (from header in headers
                select new HeaderDto
                {
                    Key = header.HeaderName,
                    Value = header.HeaderValue ?? string.Empty,
                    IsRemoval = header.Behavior == CustomHeaderBehavior.Remove
                }).ToList();
    }

    public async Task SaveAsync(ICustomHeader? model, string? modifiedBy)
    {
        if (model is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await repository.SaveAsync(model, modifiedBy);

        cache.RemoveAll();
    }

    public async Task DeleteAsync(Guid id)
    {
        await repository.DeleteAsync(id);

        cache.RemoveAll();
    }
}
