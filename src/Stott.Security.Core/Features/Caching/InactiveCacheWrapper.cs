namespace Stott.Security.Core.Features.Caching;

public class InactiveCacheWrapper : ICacheWrapper
{
    public void Add<T>(string cacheKey, T objectToCache) where T : class
    {
        return;
    }

    public T Get<T>(string cacheKey) where T : class
    {
        return null;
    }

    public void Remove(string cacheKey)
    {
        return;
    }
}