using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using TasteBox.Abstractions;
using TasteBox.Helpers;

namespace TasteBox.Services;

public class PasswordResetService(
    UserManager<ApplicationUser> userManager,
    ICacheService cacheService,
    IEmailSender emailSender,
    ILogger<PasswordResetService> logger) : IPasswordResetService
{
    private const int OtpLength = 6;
    private const int OtpExpirationMinutes = 5;
    private const int ResetTokenExpirationMinutes = 10;
    private const int RateLimitWindowMinutes = 1;
    private const int MaxOtpRequestsPerWindow = 3;

    private static string GetOtpCacheKey(string email) => $"password_reset_otp:{email.ToLowerInvariant()}";
    private static string GetResetTokenCacheKey(string email) => $"password_reset_token:{email.ToLowerInvariant()}";
    private static string GetRateLimitCacheKey(string email) => $"password_reset_rate_limit:{email.ToLowerInvariant()}";

    public async Task<Result> SendResetPasswordOtpAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        // Return success even if user not found to prevent email enumeration
        if (user is null)
        {
            logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
            return Result.Success();
        }

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        // Check rate limiting
        var rateLimitResult = await CheckRateLimitAsync(email, cancellationToken);
        if (!rateLimitResult.IsSuccess)
            return rateLimitResult;

        // Generate OTP
        var otp = GenerateOtp();
        var hashedOtp = HashOtp(otp);

        // Store hashed OTP in cache
        var otpData = new OtpCacheData
        {
            HashedOtp = hashedOtp,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

        await cacheService.SetAsync(
            GetOtpCacheKey(email),
            otpData,
            TimeSpan.FromMinutes(OtpExpirationMinutes),
            cancellationToken);

        // Send OTP via email
        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            new Dictionary<string, string>
            {
                { "{{OTP}}", otp },
                { "{{ExpirationMinutes}}", OtpExpirationMinutes.ToString() }
            });

        await emailSender.SendEmailAsync(
            user.Email!,
            "Password Reset OTP",
            emailBody);

        logger.LogInformation("Password reset OTP sent to: {Email}", email);

        return Result.Success();
    }

    public async Task<Result<VerifyResetPasswordOtpResponse>> VerifyResetPasswordOtpAsync(
        string email,
        string otp,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetOtpCacheKey(email);
        var otpData = await cacheService.GetAsync<OtpCacheData>(cacheKey, cancellationToken);

        if (otpData is null)
            return Result.Failure<VerifyResetPasswordOtpResponse>(UserErrors.InvalidOtp);

        // Check if OTP was already used
        if (otpData.IsUsed)
            return Result.Failure<VerifyResetPasswordOtpResponse>(UserErrors.InvalidOtp);

        // Verify OTP hash
        var hashedInputOtp = HashOtp(otp);
        if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hashedInputOtp),
            Encoding.UTF8.GetBytes(otpData.HashedOtp)))
        {
            return Result.Failure<VerifyResetPasswordOtpResponse>(UserErrors.InvalidOtp);
        }

        // Mark OTP as used
        otpData.IsUsed = true;
        await cacheService.SetAsync(
            cacheKey,
            otpData,
            TimeSpan.FromMinutes(1), // Keep for a short time to prevent immediate reuse
            cancellationToken);

        // Generate reset token
        var resetToken = GenerateResetToken();
        var hashedResetToken = HashOtp(resetToken);

        var resetTokenData = new ResetTokenCacheData
        {
            HashedToken = hashedResetToken,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

        await cacheService.SetAsync(
            GetResetTokenCacheKey(email),
            resetTokenData,
            TimeSpan.FromMinutes(ResetTokenExpirationMinutes),
            cancellationToken);

        logger.LogInformation("Password reset OTP verified for: {Email}", email);

        return Result.Success(new VerifyResetPasswordOtpResponse(resetToken));
    }

    public async Task<Result> ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetResetTokenCacheKey(email);
        var tokenData = await cacheService.GetAsync<ResetTokenCacheData>(cacheKey, cancellationToken);

        if (tokenData is null)
            return Result.Failure(UserErrors.InvalidResetToken);

        if (tokenData.IsUsed)
            return Result.Failure(UserErrors.InvalidResetToken);

        // Verify reset token hash
        var hashedInputToken = HashOtp(resetToken);
        if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hashedInputToken),
            Encoding.UTF8.GetBytes(tokenData.HashedToken)))
        {
            return Result.Failure(UserErrors.InvalidResetToken);
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        // Reset password
        var identityResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await userManager.ResetPasswordAsync(user, identityResetToken, newPassword);

        if (!resetResult.Succeeded)
        {
            var error = resetResult.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        // Clear cached tokens
        await cacheService.RemoveAsync(cacheKey, cancellationToken);
        await cacheService.RemoveAsync(GetOtpCacheKey(email), cancellationToken);

        logger.LogInformation("Password reset successfully for: {Email}", email);

        return Result.Success();
    }

    private async Task<Result> CheckRateLimitAsync(string email, CancellationToken cancellationToken)
    {
        var rateLimitKey = GetRateLimitCacheKey(email);
        var requestCount = await cacheService.GetAsync<int?>(rateLimitKey, cancellationToken) ?? 0;

        if (requestCount >= MaxOtpRequestsPerWindow)
            return Result.Failure(UserErrors.TooManyOtpRequests);

        await cacheService.SetAsync(
            rateLimitKey,
            requestCount + 1,
            TimeSpan.FromMinutes(RateLimitWindowMinutes),
            cancellationToken);

        return Result.Success();
    }

    private static string GenerateOtp()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }

    private static string GenerateResetToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    private static string HashOtp(string otp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
        return Convert.ToHexString(bytes);
    }
}

internal class OtpCacheData
{
    public string HashedOtp { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
}

internal class ResetTokenCacheData
{
    public string HashedToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
}
