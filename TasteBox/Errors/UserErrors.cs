using TasteBox.Abstractions;

namespace TasteBox.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);

    public static readonly Error DisabledUser =
        new("User.DisabledUser", "Disabled user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error LockedUser =
        new("User.LockedUser", "Locked user, please contact your administrator", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "Another user with the same email is already exists",
            StatusCodes.Status409Conflict);

    public static readonly Error DuplicatedUserName =
        new("User.DuplicatedUserName", "Another user with the same user name is already exists",
            StatusCodes.Status409Conflict);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);

    public static readonly Error UserNotFound =
        new("User.UserNotFound", "User is not found", StatusCodes.Status404NotFound);

    public static readonly Error InvalidRoles =
        new("Role.InvalidRoles", "Invalid roles", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidOtp =
        new("User.InvalidOtp", "Invalid or expired OTP", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidResetToken =
        new("User.InvalidResetToken", "Invalid or expired reset token", StatusCodes.Status400BadRequest);

    public static readonly Error TooManyOtpRequests =
        new("User.TooManyOtpRequests", "Too many OTP requests. Please try again later",
            StatusCodes.Status429TooManyRequests);

    public static readonly Error EmailAlreadyConfirmed =
        new("User.EmailAlreadyConfirmed", "Email is already confirmed", StatusCodes.Status400BadRequest);

    public static readonly Error OtpExpired =
        new("User.OtpExpired", "OTP has expired. Please request a new one", StatusCodes.Status400BadRequest);
}