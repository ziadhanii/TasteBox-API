namespace TasteBox.Contracts.Users;

public record UpdateProfileRequest(
    string FirstName,
    string LastName
);