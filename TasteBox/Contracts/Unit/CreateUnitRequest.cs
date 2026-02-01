namespace TasteBox.Contracts.Unit;

public record CreateUnitRequest(
    string Name,
    string Symbol,
    UnitType Type,
    bool IsBaseUnit,
    decimal ConversionFactorToBaseUnit
);
