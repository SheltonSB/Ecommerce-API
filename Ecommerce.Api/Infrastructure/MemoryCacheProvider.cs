using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Api.Infrastructure;

public class MemoryCacheProvider : ICacheProvider
{
    private readonly IMemoryCache _cache;

    public MemoryCacheProvider(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, bool trackKey = false)
    {
        _cache.Set(key, value, ttl);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix)
    {
        // MemoryCache does not support enumeration of keys directly; for simplicity clear the cache scope.
        _cache.Dispose();
        return Task.CompletedTask;
    }
}
