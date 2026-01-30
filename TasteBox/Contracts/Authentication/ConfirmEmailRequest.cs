namespace TasteBox.Contracts.Authentication;

public record ConfirmEmailRequest(
    string UserId,
    string Code
);