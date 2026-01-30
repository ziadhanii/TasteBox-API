using System.Security.Cryptography;
using TasteBox.Abstractions;

namespace TasteBox.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IOtpService otpService,
    ApplicationDbContext context
) : IAuthService
{
    private const int RefreshTokenExpiryDays = 14;

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExists = await userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
            return await otpService.SendOtpAsync(user.Email!);

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (!user.EmailConfirmed)
            return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await signInManager.PasswordSignInAsync(user, password, false, true);

        if (!result.Succeeded)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var (roles, permissions) = await GetUserRolesAndPermissions(user, cancellationToken);

        var (token, expiresIn) = jwtProvider.GenerateToken(user, roles, permissions);

        var refreshToken = GenerateRefreshToken();
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
            refreshTokenExpiration
        );

        return Result.Success(response);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (user.LockoutEnd > DateTimeOffset.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);

        var (newToken, expiresIn) = jwtProvider.GenerateToken(user, userRoles, userPermissions);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn,
            newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> VerifyOtpAsync(string email, string code)
    {
        var result = await otpService.VerifyOtpAsync(email, code);

        if (!result.IsSuccess)
            return Result.Failure(UserErrors.InvalidCode);

        var user = result.Value;

        user!.EmailConfirmed = true;
        await userManager.UpdateAsync(user);

        await userManager.AddToRoleAsync(user, DefaultRoles.Customer);

        return Result.Success();
    }

    public async Task<Result> ResendOtpAsync(string email)
        => await otpService.SendOtpAsync(email);

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(
        ApplicationUser user, CancellationToken cancellationToken)
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
}