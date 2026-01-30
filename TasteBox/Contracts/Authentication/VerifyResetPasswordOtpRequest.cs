namespace TasteBox.Contracts.Authentication;

public record VerifyResetPasswordOtpRequest(string Email, string Code);
