namespace TasteBox.Contracts.Users;

public record UserProfileResponse(
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string PhoneNumber
);