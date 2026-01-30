using TasteBox.Abstractions;

namespace TasteBox.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password,
        CancellationToken cancellationToken = default);

    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> VerifyOtpAsync(string email, string code);

    Task<Result> ResendOtpAsync(string email);
}