using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace TasteBox.Services;

/// <summary>
/// Provides distributed cache operations with JSON serialization
/// </summary>
public sealed class CacheService(IDistributedCache cache) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        var serializedValue = JsonSerializer.Serialize(value, JsonOptions);
        await cache.SetStringAsync(key, serializedValue, options, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var value = await cache.GetStringAsync(key, cancellationToken);

        return string.IsNullOrEmpty(value)
            ? default
            : JsonSerializer.Deserialize<T>(value, JsonOptions);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        await cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var value = await cache.GetStringAsync(key, cancellationToken);
        return !string.IsNullOrEmpty(value);
    }
}
