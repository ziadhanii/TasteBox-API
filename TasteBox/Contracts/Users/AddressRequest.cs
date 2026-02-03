namespace TasteBox.Contracts.Users;

public record AddressRequest(
    string Street,
    string City,
    string State,
    string ZipCode,
    string? BuildingNumber = null,
    string? Floor = null,
    string? Apartment = null,
    string? Landmark = null,
    string? PhoneNumber = null,
    bool IsDefault = false
);
