namespace TasteBox.Contracts.Users;

public record AddressResponse(
    int Id,
    string Street,
    string City,
    string State,
    string ZipCode,
    string? BuildingNumber,
    string? Floor,
    string? Apartment,
    string? Landmark,
    string? PhoneNumber,
    bool IsDefault
);
