namespace Stott.Security.Optimizely.Features.Caching;

using System;
using System.Linq;

using EPiServer.Framework.Cache;
using EPiServer.Logging;

using Stott.Security.Optimizely.Common;

public sealed class CacheWrapper : ICacheWrapper
{
    private readonly ISynchronizedObjectInstanceCache _cache;

    private readonly ILogger _logger = LogManager.GetLogger(typeof(CacheWrapper));

    private const string MasterKey = "Stott-Security-MasterKey";

    public CacheWrapper(ISynchronizedObjectInstanceCache cache)
    {
        _cache = cache;
    }

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
                new[] { MasterKey });

            _cache.Insert(cacheKey, objectToCache, evictionPolicy);
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Failed to add item to cache with a key of {cacheKey}.", exception);
        }
    }

    public T? Get<T>(string cacheKey)
        where T : class
    {
        return _cache.TryGet<T>(cacheKey, ReadStrategy.Wait, out var cachedObject) ? cachedObject : default;
    }

    public void RemoveAll()
    {
        try
        {
            _cache.Remove(MasterKey);
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Failed to remove all items from cache based on the master key.", exception);
        }
    }
}