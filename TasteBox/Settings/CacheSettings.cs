namespace TasteBox.Settings;

public class CacheSettings
{
    public const string SectionName = "CacheSettings";
    public required string Host { get; set; }
    public int Port { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }
}