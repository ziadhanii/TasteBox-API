namespace TasteBox.Contracts.Unit;

public record UpdateUnitRequest(
    string Name,
    string Symbol,
    UnitType Type,
    bool IsBaseUnit,
    decimal ConversionFactorToBaseUnit
);
