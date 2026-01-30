using Microsoft.AspNetCore.Identity.UI.Services;
using TasteBox.Abstractions;
using TasteBox.Helpers;
using TasteBox.Helpers.CacheData;

namespace TasteBox.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IOtpService otpService,
    ICacheService cacheService,
    IEmailSender emailSender,
    EmailBodyBuilder emailBodyBuilder,
    ApplicationDbContext context) : IAuthService
{
    private const int RefreshTokenExpiryDays = 14;
    private const int OtpExpirationMinutes = 5;
    private const int ResetTokenExpirationMinutes = 10;
    private const int RateLimitWindowMinutes = 1;
    private const int MaxRequestsPerWindow = 3;
    private const int UsedTokenRetentionMinutes = 1;

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await EmailExistsAsync(request.Email, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return CreateIdentityError(result);

        return await otpService.SendOtpAsync(user.Email!);
    }

    public async Task<Result> VerifyEmailAsync(string email, string code)
    {
        var result = await otpService.VerifyOtpAsync(email, code);
        if (!result.IsSuccess)
            return Result.Failure(UserErrors.InvalidCode);

        var user = result.Value!;
        user.EmailConfirmed = true;

        await userManager.UpdateAsync(user);
        await userManager.AddToRoleAsync(user, DefaultRoles.Customer);

        return Result.Success();
    }

    public async Task<Result> ResendEmailVerificationOtpAsync(string email)
        => await otpService.SendOtpAsync(email);

    public async Task<Result<AuthResponse>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        var validationResult = ValidateUserForLogin(user);
        if (!validationResult.IsSuccess)
            return Result.Failure<AuthResponse>(validationResult.Error);

        var signInResult = await signInManager.PasswordSignInAsync(user!, password, false, true);
        if (!signInResult.Succeeded)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        return await GenerateAuthResponseAsync(user!, cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshAccessTokenAsync(
        string token,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var user = await ValidateAndGetUserFromTokenAsync(token);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var validationResult = ValidateUserStatus(user);
        if (!validationResult.IsSuccess)
            return Result.Failure<AuthResponse>(validationResult.Error);

        var userRefreshToken = GetActiveRefreshToken(user, refreshToken);
        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        return await GenerateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        string token,
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var user = await ValidateAndGetUserFromTokenAsync(token);
        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var userRefreshToken = GetActiveRefreshToken(user, refreshToken);
        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> SendResetPasswordOtpAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        // Return success even for non-existent emails (security best practice)
        if (user is null)
        {
            logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
            return Result.Success();
        }

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var rateLimitResult = await EnforceRateLimitAsync(email, cancellationToken);
        if (!rateLimitResult.IsSuccess)
            return rateLimitResult;

        var otp = CryptoHelper.GenerateOtp();
        await StorePasswordResetOtpAsync(email, otp, cancellationToken);
        await SendPasswordResetEmailAsync(user.Email!, otp);

        logger.LogInformation("Password reset OTP sent to: {Email}", email);

        return Result.Success();
    }

    public async Task<Result<VerifyResetPasswordOtpResponse>> VerifyResetPasswordOtpAsync(
        string email,
        string otp,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.PasswordResetOtp(email);
        var otpData = await cacheService.GetAsync<PasswordResetOtpData>(cacheKey, cancellationToken);

        if (otpData is null || otpData.IsUsed)
            return Result.Failure<VerifyResetPasswordOtpResponse>(UserErrors.InvalidOtp);

        if (!CryptoHelper.VerifyHash(otp, otpData.HashedOtp))
            return Result.Failure<VerifyResetPasswordOtpResponse>(UserErrors.InvalidOtp);

        await MarkOtpAsUsedAsync(cacheKey, otpData, cancellationToken);

        var resetToken = CryptoHelper.GenerateResetToken();
        await StoreResetTokenAsync(email, resetToken, cancellationToken);

        logger.LogInformation("Password reset OTP verified for: {Email}", email);

        return Result.Success(new VerifyResetPasswordOtpResponse(resetToken));
    }

    public async Task<Result> ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.PasswordResetToken(email);
        var tokenData = await cacheService.GetAsync<PasswordResetTokenData>(cacheKey, cancellationToken);

        if (tokenData is null || tokenData.IsUsed)
            return Result.Failure(UserErrors.InvalidResetToken);

        if (!CryptoHelper.VerifyHash(resetToken, tokenData.HashedToken))
            return Result.Failure(UserErrors.InvalidResetToken);

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);

        var resetResult = await ResetUserPasswordAsync(user, newPassword);
        if (!resetResult.IsSuccess)
            return resetResult;

        await ClearPasswordResetCacheAsync(email, cancellationToken);

        logger.LogInformation("Password reset successfully for: {Email}", email);

        return Result.Success();
    }


    private async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        => await userManager.Users.AnyAsync(x => x.Email == email, cancellationToken);

    private async Task<ApplicationUser?> ValidateAndGetUserFromTokenAsync(string token)
    {
        var userId = jwtProvider.ValidateToken(token);
        return userId is null ? null : await userManager.FindByIdAsync(userId);
    }

    private static Result ValidateUserForLogin(ApplicationUser? user)
    {
        if (user is null)
            return Result.Failure(UserErrors.InvalidCredentials);

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        if (user.IsDisabled)
            return Result.Failure(UserErrors.DisabledUser);

        return Result.Success();
    }

    private static Result ValidateUserStatus(ApplicationUser user)
    {
        if (user.IsDisabled)
            return Result.Failure(UserErrors.DisabledUser);

        if (user.LockoutEnd > DateTimeOffset.UtcNow)
            return Result.Failure(UserErrors.LockedUser);

        return Result.Success();
    }


    private static RefreshToken? GetActiveRefreshToken(ApplicationUser user, string refreshToken)
        => user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

    private async Task<Result<AuthResponse>> GenerateAuthResponseAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user, cancellationToken);
        var (token, expiresIn) = jwtProvider.GenerateToken(user, roles, permissions);

        var refreshToken = CryptoHelper.GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await userManager.UpdateAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            token,
            expiresIn,
            refreshToken,
            refreshTokenExpiration);

        return Result.Success(response);
    }

    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissionsAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        var userRoles = await userManager.GetRolesAsync(user);

        var userPermissions = await (
            from r in context.Roles
            join p in context.RoleClaims on r.Id equals p.RoleId
            where userRoles.Contains(r.Name!)
            select p.ClaimValue!
        ).Distinct().ToListAsync(cancellationToken);

        return (userRoles, userPermissions);
    }


    private async Task<Result> EnforceRateLimitAsync(string email, CancellationToken cancellationToken)
    {
        var rateLimitKey = CacheKeys.PasswordResetRateLimit(email);
        var requestCount = await cacheService.GetAsync<int?>(rateLimitKey, cancellationToken) ?? 0;

        if (requestCount >= MaxRequestsPerWindow)
            return Result.Failure(UserErrors.TooManyOtpRequests);

        await cacheService.SetAsync(
            rateLimitKey,
            requestCount + 1,
            TimeSpan.FromMinutes(RateLimitWindowMinutes),
            cancellationToken);

        return Result.Success();
    }


    private async Task StorePasswordResetOtpAsync(string email, string otp, CancellationToken cancellationToken)
    {
        var otpData = new PasswordResetOtpData
        {
            HashedOtp = CryptoHelper.ComputeHash(otp),
            Email = email
        };

        await cacheService.SetAsync(
            CacheKeys.PasswordResetOtp(email),
            otpData,
            TimeSpan.FromMinutes(OtpExpirationMinutes),
            cancellationToken);
    }

    private async Task MarkOtpAsUsedAsync(
        string cacheKey,
        PasswordResetOtpData otpData,
        CancellationToken cancellationToken)
    {
        otpData.IsUsed = true;
        await cacheService.SetAsync(
            cacheKey,
            otpData,
            TimeSpan.FromMinutes(UsedTokenRetentionMinutes),
            cancellationToken);
    }

    private async Task StoreResetTokenAsync(string email, string resetToken, CancellationToken cancellationToken)
    {
        var resetTokenData = new PasswordResetTokenData
        {
            HashedToken = CryptoHelper.ComputeHash(resetToken),
            Email = email
        };

        await cacheService.SetAsync(
            CacheKeys.PasswordResetToken(email),
            resetTokenData,
            TimeSpan.FromMinutes(ResetTokenExpirationMinutes),
            cancellationToken);
    }

    private async Task ClearPasswordResetCacheAsync(string email, CancellationToken cancellationToken)
    {
        await cacheService.RemoveAsync(CacheKeys.PasswordResetToken(email), cancellationToken);
        await cacheService.RemoveAsync(CacheKeys.PasswordResetOtp(email), cancellationToken);
    }


    private async Task SendPasswordResetEmailAsync(string email, string otp)
    {
        var emailBody = emailBodyBuilder.GenerateEmailBody("ForgetPassword",
            new Dictionary<string, string>
            {
                { "{{OTP}}", otp },
                { "{{ExpirationMinutes}}", OtpExpirationMinutes.ToString() }
            });

        await emailSender.SendEmailAsync(email, "Password Reset OTP", emailBody);
    }

    private async Task<Result> ResetUserPasswordAsync(ApplicationUser user, string newPassword)
    {
        var identityResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await userManager.ResetPasswordAsync(user, identityResetToken, newPassword);

        return resetResult.Succeeded
            ? Result.Success()
            : CreateIdentityError(resetResult);
    }


    private static Result CreateIdentityError(IdentityResult result)
    {
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}