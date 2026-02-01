namespace TasteBox.Contracts.Unit;

public record UnitResponse(
    int Id,
    string Name,
    string Symbol,
    UnitType Type,
    bool IsBaseUnit,
    decimal ConversionFactorToBaseUnit
);