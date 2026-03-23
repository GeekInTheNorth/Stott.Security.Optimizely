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
internal sealed class CustomHeaderService(ICustomHeaderRepository repository, ICacheWrapper cache) : ICustomHeaderService
{
    private const string ListCacheKeyPrefix = "stott.security.customheaders.list";

    private const string CompiledCacheKeyPrefix = "stott.security.customheaders.compiled";

    public async Task<IList<CustomHeaderModel>> GetAllAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(ListCacheKeyPrefix, appId, hostName);
        var cachedHeaders = cache.Get<List<CustomHeaderModel>>(cacheKey);
        if (cachedHeaders is not null)
        {
            return cachedHeaders;
        }

        var headers = await repository.GetAllAsync(appId, hostName);
        var models = headers.Select(CustomHeaderMapper.ToModel).ToList();
        models.AddRange(GetDefaultHeaders(models));

        cache.Add(cacheKey, models);

        return models;
    }

    public async Task<IList<HeaderDto>> GetCompiledHeaders(string? appId, string? hostName)
    {
        var headers = await GetAllAsync(appId, hostName);
        return (from header in headers
                where header.Behavior != CustomHeaderBehavior.Disabled
                select new HeaderDto
                {
                    Key = header.HeaderName,
                    Value = header.HeaderValue ?? string.Empty,
                    IsRemoval = header.Behavior == CustomHeaderBehavior.Remove
                }).ToList();
    }

    public async Task<bool> HasOverrideAsync(string? appId, string? hostName)
    {
        var headers = await repository.GetAllByContextAsync(appId, hostName);
        return headers is not null;
    }

    public async Task CreateOverrideAsync(string? appId, string? hostName, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        // Determine the source context to copy from (the parent in the fallback chain)
        string? sourceAppId = null;
        string? sourceHostName = null;

        if (!string.IsNullOrWhiteSpace(hostName))
        {
            // Creating host-level override: source is app-level (or global)
            sourceAppId = appId;
            sourceHostName = null;
        }

        // For app-level override: source is global (null, null) which is the default

        await repository.CreateOverrideAsync(sourceAppId, sourceHostName, appId, hostName, modifiedBy);

        cache.RemoveAll();
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy)) throw new ArgumentNullException(nameof(deletedBy));

        await repository.DeleteByContextAsync(appId, hostName, deletedBy);

        cache.RemoveAll();
    }

    public async Task SaveAsync(ICustomHeader? model, string? modifiedBy, string? appId, string? hostName)
    {
        if (model is null || string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await repository.SaveAsync(model, modifiedBy, appId, hostName);

        cache.RemoveAll();
    }

    public async Task DeleteAsync(Guid id)
    {
        await repository.DeleteAsync(id);

        cache.RemoveAll();
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

    private static string GetCacheKey(string prefix, string? appId, string? hostName)
    {
        return $"{prefix}.{appId ?? "global"}.{hostName ?? "all"}";
    }
}
