namespace TasteBox.Helpers.CacheData;

public sealed class PasswordResetOtpData
{
    public required string HashedOtp { get; init; }
    public required string Email { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsUsed { get; set; }
}
