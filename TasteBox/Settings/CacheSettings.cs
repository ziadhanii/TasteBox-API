namespace TasteBox.Settings;

public class CacheSettings
{
    public const string SectionName = "CacheSettings";
    
    /// <summary>
    /// Type of cache provider: "InMemory" or "Redis"
    /// </summary>
    public string Provider { get; set; } = "InMemory";
    
    /// <summary>
    /// Redis connection string (required if Provider is "Redis")
    /// </summary>
    public string? RedisConnectionString { get; set; }
    
    /// <summary>
    /// Redis instance name prefix
    /// </summary>
    public string RedisInstanceName { get; set; } = "TasteBox:";
}
