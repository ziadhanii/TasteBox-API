using Microsoft.AspNetCore.Identity.UI.Services;
using TasteBox.Abstractions;
using TasteBox.Helpers;
using TasteBox.Helpers.CacheData;


namespace TasteBox.Services;

public sealed class OtpService(
    UserManager<ApplicationUser> userManager,
    ICacheService cacheService,
    IEmailSender emailSender,
    EmailBodyBuilder emailBodyBuilder,
    ILogger<OtpService> logger) : IOtpService
{
    private const int OtpExpirationMinutes = 5;
    private const int RateLimitWindowMinutes = 1;
    private const int MaxRequestsPerWindow = 3;
    private const int UsedOtpRetentionMinutes = 1;


    public async Task<Result> SendOtpAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCredentials);

        var rateLimitResult = await EnforceRateLimitAsync(email);
        if (!rateLimitResult.IsSuccess)
            return rateLimitResult;

        var otp = CryptoHelper.GenerateOtp();
        await StoreOtpAsync(email, user.Id, otp);
        await SendOtpEmailAsync(user.Email!, otp);

        logger.LogInformation("Email verification OTP sent to: {Email}", email);

        return Result.Success();
    }

    public async Task<Result<ApplicationUser>> VerifyOtpAsync(string email, string code)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure<ApplicationUser>(UserErrors.InvalidCredentials);

        var cacheKey = CacheKeys.EmailVerificationOtp(email);
        var otpData = await cacheService.GetAsync<EmailVerificationOtpData>(cacheKey);

        if (otpData is null || otpData.IsUsed)
            return Result.Failure<ApplicationUser>(UserErrors.InvalidOtp);

        if (!CryptoHelper.VerifyHash(code, otpData.HashedOtp))
            return Result.Failure<ApplicationUser>(UserErrors.InvalidCode);

        await MarkOtpAsUsedAsync(cacheKey, otpData);
        await ClearRateLimitAsync(email);

        logger.LogInformation("Email verification OTP verified for: {Email}", email);

        return Result.Success(user);
    }


    private async Task<Result> EnforceRateLimitAsync(string email)
    {
        var rateLimitKey = CacheKeys.EmailVerificationRateLimit(email);
        var requestCount = await cacheService.GetAsync<int?>(rateLimitKey) ?? 0;

        if (requestCount >= MaxRequestsPerWindow)
            return Result.Failure(UserErrors.TooManyOtpRequests);

        await cacheService.SetAsync(
            rateLimitKey,
            requestCount + 1,
            TimeSpan.FromMinutes(RateLimitWindowMinutes));

        return Result.Success();
    }

    private async Task StoreOtpAsync(string email, string userId, string otp)
    {
        var otpData = new EmailVerificationOtpData
        {
            HashedOtp = CryptoHelper.ComputeHash(otp),
            UserId = userId,
            Email = email
        };

        await cacheService.SetAsync(
            CacheKeys.EmailVerificationOtp(email),
            otpData,
            TimeSpan.FromMinutes(OtpExpirationMinutes));
    }

    private async Task SendOtpEmailAsync(string email, string otp)
    {
        var emailBody = emailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string>
            {
                { "{{OTP}}", otp },
                { "{{ExpirationMinutes}}", OtpExpirationMinutes.ToString() }
            });

        await emailSender.SendEmailAsync(email, "Email Confirmation OTP", emailBody);
    }

    private async Task MarkOtpAsUsedAsync(string cacheKey, EmailVerificationOtpData otpData)
    {
        otpData.IsUsed = true;
        await cacheService.SetAsync(
            cacheKey,
            otpData,
            TimeSpan.FromMinutes(UsedOtpRetentionMinutes));
    }

    private async Task ClearRateLimitAsync(string email)
    {
        await cacheService.RemoveAsync(CacheKeys.EmailVerificationRateLimit(email));
    }
}