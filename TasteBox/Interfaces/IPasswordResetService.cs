using TasteBox.Abstractions;

namespace TasteBox.Interfaces;

public interface IPasswordResetService
{
    /// <summary>
    /// Sends a password reset OTP to the user's email
    /// </summary>
    Task<Result> SendResetPasswordOtpAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the OTP and returns a reset token if valid
    /// </summary>
    Task<Result<VerifyResetPasswordOtpResponse>> VerifyResetPasswordOtpAsync(
        string email, 
        string otp, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the password using the reset token
    /// </summary>
    Task<Result> ResetPasswordAsync(
        string email, 
        string resetToken, 
        string newPassword, 
        CancellationToken cancellationToken = default);
}
