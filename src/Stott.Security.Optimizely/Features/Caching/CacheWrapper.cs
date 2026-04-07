namespace Stott.Security.Optimizely.Features.Caching;

using System;
using System.Linq;
using EPiServer.Framework.Cache;
using Microsoft.Extensions.Logging;
using Stott.Security.Optimizely.Common;

public sealed class CacheWrapper(ISynchronizedObjectInstanceCache cache, ILogger<CacheWrapper> logger) : ICacheWrapper
{
    public void Add<T>(string cacheKey, T? objectToCache)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(cacheKey) || objectToCache == null)
        {
            return;
        }

        try
        {
            var evictionPolicy = new CacheEvictionPolicy(
                TimeSpan.FromHours(12),
                CacheTimeoutType.Absolute,
                Enumerable.Empty<string>(),
                new[] { CspConstants.CacheKeys.MasterKey });

            cache.Insert(cacheKey, objectToCache, evictionPolicy);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{prefix} Failed to add item to cache with a key of {cacheKey}.", CspConstants.LogPrefix, cacheKey);
        }
    }

    public T? Get<T>(string cacheKey)
        where T : class
    {
        return cache.TryGet<T>(cacheKey, ReadStrategy.Immediate, out var cachedObject) ? cachedObject : default;
    }

    public void RemoveAll()
    {
        try
        {
            cache.Remove(CspConstants.CacheKeys.MasterKey);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{prefix} Failed to remove all items from cache based on the master key.", CspConstants.LogPrefix);
        }
    }
}