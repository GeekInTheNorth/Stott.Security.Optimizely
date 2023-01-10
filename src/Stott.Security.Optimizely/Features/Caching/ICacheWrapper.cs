namespace Stott.Security.Optimizely.Features.Caching;

public interface ICacheWrapper
{
    void Add<T>(string cacheKey, T? objectToCache) where T : class;

    T? Get<T>(string cacheKey) where T : class;

    void Remove(string cacheKey);
}
