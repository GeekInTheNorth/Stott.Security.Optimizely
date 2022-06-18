namespace Stott.Security.Optimizely.Features.Caching;

using EPiServer.Framework.Cache;
using EPiServer.Logging;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Caching;

using System;

public class CacheWrapper : ICacheWrapper
{
    private readonly ISynchronizedObjectInstanceCache _cache;

    private readonly ILogger _logger = LogManager.GetLogger(typeof(CacheWrapper));

    public CacheWrapper(ISynchronizedObjectInstanceCache cache)
    {
        _cache = cache;
    }

    public void Add<T>(string cacheKey, T objectToCache)
        where T : class
    {
        if (!string.IsNullOrWhiteSpace(cacheKey) && objectToCache != null)
        {
            try
            {
                var evictionPolicy = new CacheEvictionPolicy(TimeSpan.FromHours(1), CacheTimeoutType.Absolute);

                _cache.Insert(cacheKey, objectToCache, evictionPolicy);
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to add item to cache with a key of {cacheKey}.", exception);
            }
        }
    }

    public T Get<T>(string cacheKey)
        where T : class
    {
        if (_cache.TryGet<T>(cacheKey, ReadStrategy.Wait, out var cachedObject))
        {
            return cachedObject;
        }

        return null;
    }

    public void Remove(string cacheKey)
    {
        if (!string.IsNullOrWhiteSpace(cacheKey))
        {
            try
            {
                _cache.Remove(cacheKey);
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to remove item from cache with a key of {cacheKey}.", exception);
            }
        }
    }
}
