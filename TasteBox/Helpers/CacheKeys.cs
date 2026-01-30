namespace TasteBox.Helpers;

public static class CacheKeys
{
    private const string EmailVerificationOtpPrefix = "email_verification:otp";
    private const string EmailVerificationRateLimitPrefix = "email_verification:rate_limit";

    private const string PasswordResetOtpPrefix = "password_reset:otp";
    private const string PasswordResetTokenPrefix = "password_reset:token";
    private const string PasswordResetRateLimitPrefix = "password_reset:rate_limit";

    public static string EmailVerificationOtp(string email)
        => $"{EmailVerificationOtpPrefix}:{NormalizeEmail(email)}";

    public static string EmailVerificationRateLimit(string email)
        => $"{EmailVerificationRateLimitPrefix}:{NormalizeEmail(email)}";

    public static string PasswordResetOtp(string email)
        => $"{PasswordResetOtpPrefix}:{NormalizeEmail(email)}";

    public static string PasswordResetToken(string email)
        => $"{PasswordResetTokenPrefix}:{NormalizeEmail(email)}";

    public static string PasswordResetRateLimit(string email)
        => $"{PasswordResetRateLimitPrefix}:{NormalizeEmail(email)}";

    private static string NormalizeEmail(string email)
        => email.ToLowerInvariant();
}