namespace Ecommerce.Api.Infrastructure;

public interface ICacheProvider
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan ttl, bool trackKey = false);
    Task RemoveAsync(string key);
    Task RemoveByPrefixAsync(string prefix);
}
