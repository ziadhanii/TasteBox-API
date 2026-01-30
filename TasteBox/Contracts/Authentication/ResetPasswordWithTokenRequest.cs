namespace TasteBox.Contracts.Authentication;

public record ResetPasswordWithTokenRequest(
    string Email,
    string ResetToken,
    string NewPassword
);