namespace Stott.Security.Optimizely.Features.CustomHeaders.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;
using Stott.Security.Optimizely.Features.Header;

/// <summary>
/// Service implementation for custom header business logic.
/// </summary>
internal sealed class CustomHeaderService : ICustomHeaderService
{
    private readonly ICustomHeaderRepository _repository;

    private readonly ICacheWrapper _cache;

    public CustomHeaderService(ICustomHeaderRepository repository, ICacheWrapper cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IList<CustomHeaderModel>> GetAllAsync(Guid? siteId, string? hostName)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CustomHeaders, siteId, hostName);
        var cachedHeaders = _cache.Get<List<CustomHeaderModel>>(cacheKey);
        if (cachedHeaders is not null)
        {
            return cachedHeaders;
        }

        var headers = await _repository.GetAllAsync(siteId, hostName);
        var models = headers.Select(CustomHeaderMapper.ToModel).ToList();
        models.AddRange(GetDefaultHeaders(models));

        _cache.Add(cacheKey, models);

        return models;
    }

    public async Task<IList<HeaderDto>> GetCompiledHeaders(Guid? siteId, string? hostName)
    {
        var headers = await GetAllAsync(siteId, hostName);
        return (from header in headers
                where header.Behavior != CustomHeaderBehavior.Disabled
                select new HeaderDto
                {
                    Key = header.HeaderName,
                    Value = header.HeaderValue ?? string.Empty,
                    IsRemoval = header.Behavior == CustomHeaderBehavior.Remove
                }).ToList();
    }

    public async Task<bool> ExistsForContextAsync(Guid? siteId, string? hostName)
    {
        var hasSiteId = siteId.HasValue && siteId.Value != Guid.Empty;
        var hasHostName = !string.IsNullOrWhiteSpace(hostName);
        if (!hasSiteId && !hasHostName)
        {
            // The Global scope always exists.
            return true;
        }

        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CustomHeadersInherited, siteId, hostName);
        var ctxState = _cache.Get<ContextStateModel>(cacheKey);
        if (ctxState is null)
        {
            var actualSettings = await _repository.GetAllByContextAsync(siteId, hostName);
            ctxState = new ContextStateModel
            {
                Exists = actualSettings?.Any() ?? false
            };

            _cache.Add(cacheKey, ctxState);
        }

        return ctxState.Exists;
    }

    public async Task CreateOverrideAsync(Guid? siteId, string? hostName, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        // Determine the source context to copy from (the parent in the fallback chain)
        Guid? sourceSiteId = null;
        string? sourceHostName = null;

        if (!string.IsNullOrWhiteSpace(hostName))
        {
            // Creating host-level override: source is site-level (or global)
            sourceSiteId = siteId;
            sourceHostName = null;
        }

        // For site-level override: source is global (null, null) which is the default

        await _repository.CreateOverrideAsync(sourceSiteId, sourceHostName, siteId, hostName, modifiedBy);

        _cache.RemoveAll();
    }

    public async Task DeleteByContextAsync(Guid? siteId, string? hostName, string? deletedBy)
    {
        if (!siteId.HasValue || siteId.Value == Guid.Empty) throw new ArgumentNullException(nameof(siteId));
        if (string.IsNullOrWhiteSpace(deletedBy)) throw new ArgumentNullException(nameof(deletedBy));

        await _repository.DeleteByContextAsync(siteId, hostName, deletedBy);

        _cache.RemoveAll();
    }

    public async Task SaveAsync(ICustomHeader? model, string? modifiedBy, Guid? siteId, string? hostName)
    {
        if (model is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.SaveAsync(model, modifiedBy, siteId, hostName);

        _cache.RemoveAll();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);

        _cache.RemoveAll();
    }

    private static List<CustomHeaderModel> GetDefaultHeaders(IList<CustomHeaderModel> models)
    {
        var defaultHeaders = new List<string>
        {
            CspConstants.HeaderNames.XssProtection,
            CspConstants.HeaderNames.FrameOptions,
            CspConstants.HeaderNames.ContentTypeOptions,
            CspConstants.HeaderNames.ReferrerPolicy,
            CspConstants.HeaderNames.CrossOriginEmbedderPolicy,
            CspConstants.HeaderNames.CrossOriginOpenerPolicy,
            CspConstants.HeaderNames.CrossOriginResourcePolicy,
            CspConstants.HeaderNames.StrictTransportSecurity,
        };

        return defaultHeaders.Where(x => !models.Any(y => string.Equals(x, y.HeaderName, StringComparison.OrdinalIgnoreCase)))
                             .Select(CustomHeaderMapper.ToModel)
                             .ToList();
    }

    private static string GetCacheKey(string prefix, Guid? siteId, string? hostName)
    {
        var sitePart = siteId.HasValue && siteId.Value != Guid.Empty ? siteId.Value.ToString("N") : "global";
        var hostPart = string.IsNullOrWhiteSpace(hostName) ? string.Empty : hostName.ToLowerInvariant();
        return $"{prefix}.{sitePart}.{hostPart}";
    }
}
