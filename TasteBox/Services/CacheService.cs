using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace TasteBox.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        var serializedValue = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, serializedValue, options, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await cache.GetStringAsync(key, cancellationToken);
        
        if (string.IsNullOrEmpty(value))
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await cache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(value);
    }
}
