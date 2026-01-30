using TasteBox.Abstractions;

namespace TasteBox.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(string email, string password,
        CancellationToken cancellationToken = default);

    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> RefreshAccessTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> VerifyEmailAsync(string email, string code);

    Task<Result> ResendEmailVerificationOtpAsync(string email);

    Task<Result> SendResetPasswordOtpAsync(string email, CancellationToken cancellationToken = default);


    Task<Result<VerifyResetPasswordOtpResponse>> VerifyResetPasswordOtpAsync(
        string email,
        string otp,
        CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default);
}