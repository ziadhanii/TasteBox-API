using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace TasteBox.Services;

public sealed class CacheService(IDistributedCache cache, IConnectionMultiplexer? connectionMultiplexer = null)
    : ICacheService
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

    public Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        => SetAsync(cacheKey, response, timeToLive);

    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cacheKey);

        var cachedData = await GetAsync<object>(cacheKey);
        return cachedData != null ? JsonSerializer.Serialize(cachedData, JsonOptions) : null;
    }

    public async Task RemoveCacheByPatternAsync(string pattern)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);

        if (connectionMultiplexer == null)
        {
            await RemoveAsync(pattern);
            return;
        }

        var database = connectionMultiplexer.GetDatabase();
        var endpoints = connectionMultiplexer.GetEndPoints();

        foreach (var endpoint in endpoints)
        {
            var server = connectionMultiplexer.GetServer(endpoint);

            var keys = server.Keys(pattern: $"*{pattern}*").ToArray();

            if (keys.Length > 0)
            {
                await database.KeyDeleteAsync(keys);
            }
        }
    }
}