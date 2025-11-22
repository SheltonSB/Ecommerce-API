using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Api.Infrastructure;

public class DistributedCacheProvider : ICacheProvider
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private const string ProductKeyRegistry = "__cachekeys:products";

    public DistributedCacheProvider(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data)) return default;
        return JsonSerializer.Deserialize<T>(data, SerializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, bool trackKey = false)
    {
        var json = JsonSerializer.Serialize(value, SerializerOptions);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });

        if (trackKey)
        {
            var keys = await GetKeyRegistry();
            if (!keys.Contains(key))
            {
                keys.Add(key);
                await SaveKeyRegistry(keys);
            }
        }
    }

    public Task RemoveAsync(string key) => _cache.RemoveAsync(key);

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var keys = await GetKeyRegistry();
        var toRemove = keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
        foreach (var key in toRemove)
        {
            await _cache.RemoveAsync(key);
            keys.Remove(key);
        }
        await SaveKeyRegistry(keys);
    }

    private async Task<List<string>> GetKeyRegistry()
    {
        var data = await _cache.GetStringAsync(ProductKeyRegistry);
        if (string.IsNullOrEmpty(data)) return new List<string>();
        return JsonSerializer.Deserialize<List<string>>(data, SerializerOptions) ?? new List<string>();
    }

    private Task SaveKeyRegistry(List<string> keys)
    {
        var json = JsonSerializer.Serialize(keys, SerializerOptions);
        return _cache.SetStringAsync(ProductKeyRegistry, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        });
    }
}
