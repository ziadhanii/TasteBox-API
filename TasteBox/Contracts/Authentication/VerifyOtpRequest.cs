namespace TasteBox.Contracts.Authentication;

public record VerifyOtpRequest(string Email, string Code);
