namespace TasteBox.Controllers;

public class AuthenticationController(
    IAuthService authService) : APIBaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Verify email address using OTP sent during registration
    /// </summary>
    [HttpPost("email/verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyOtpRequest request)
    {
        var result = await authService.VerifyEmailAsync(request.Email, request.Code);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Resend email verification OTP
    /// </summary>
    [HttpPost("email/resend-otp")]
    public async Task<IActionResult> ResendEmailVerificationOtp([FromBody] ResendOtpRequest request)
    {
        var result = await authService.ResendEmailVerificationOtpAsync(request.Email);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = await authService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshAccessToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = await authService.RefreshAccessTokenAsync(
            request.Token,
            request.RefreshToken,
            cancellationToken);

        return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RevokeRefreshTokenAsync(
            request.Token,
            request.RefreshToken,
            cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Step 1: Send password reset OTP to user's email
    /// </summary>
    [HttpPost("forgot-password/send-otp")]
    public async Task<IActionResult> SendResetPasswordOtp(
        [FromBody] SendResetPasswordOtpRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.SendResetPasswordOtpAsync(request.Email, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    /// <summary>
    /// Step 2: Verify the OTP and get a reset token
    /// </summary>
    [HttpPost("forgot-password/verify-otp")]
    public async Task<IActionResult> VerifyResetPasswordOtp(
        [FromBody] VerifyResetPasswordOtpRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.VerifyResetPasswordOtpAsync(
            request.Email,
            request.Code,
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Step 3: Reset password using the reset token
    /// </summary>
    [HttpPost("forgot-password/reset")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordWithTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.ResetPasswordAsync(
            request.Email,
            request.ResetToken,
            request.NewPassword,
            cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}