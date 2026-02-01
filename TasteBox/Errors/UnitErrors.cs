using TasteBox.Abstractions;

namespace TasteBox.Errors;

public class UnitErrors
{
    public static readonly Error UnitNotFound =
        new("Unit.NotFound", "No Unit was found with the given ID", StatusCodes.Status404NotFound);

    public static readonly Error UnitNameAlreadyExists =
        new("Unit.NameExists", "Unit name already exists", StatusCodes.Status400BadRequest);

    public static readonly Error UnitSymbolAlreadyExists =
        new("Unit.SymbolExists", "Unit symbol already exists", StatusCodes.Status400BadRequest);

    public static readonly Error CannotDeactivateUnitInUse =
        new("Unit.InUse", "Cannot deactivate unit that is currently in use", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidConversionFactor =
        new("Unit.InvalidFactor", "Conversion factor must be greater than zero", StatusCodes.Status400BadRequest);

    public static readonly Error BaseUnitConversionMustBeOne =
        new("Unit.BaseUnitFactor", "Base unit must have conversion factor of 1", StatusCodes.Status400BadRequest);

    public static readonly Error IncompatibleUnits =
        new("Unit.Incompatible", "Cannot convert between different unit types", StatusCodes.Status400BadRequest);
}