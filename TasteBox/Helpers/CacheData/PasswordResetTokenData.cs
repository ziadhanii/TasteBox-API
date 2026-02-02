namespace TasteBox.Helpers.CacheData;

public sealed class PasswordResetTokenData
{
    public required string HashedToken { get; init; }
    public required string Email { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public required DateTime ExpiresAt { get; init; }
    public bool IsUsed { get; set; }
}