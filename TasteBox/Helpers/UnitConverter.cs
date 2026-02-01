using TasteBox.Abstractions;

namespace TasteBox.Helpers;

public class UnitConverter : IUnitConverter
{
    public decimal ToBase(decimal quantity, Unit unit)
    {
        return quantity * unit.ConversionFactorToBaseUnit;
    }

    public bool CanConvert(Unit from, Unit to)
    {
        return from.Type == to.Type;
    }

    public Result<decimal> Convert(decimal quantity, Unit from, Unit to)
    {
        if (!CanConvert(from, to))
            return Result.Failure<decimal>(UnitErrors.IncompatibleUnits);

        decimal baseQuantity = quantity * from.ConversionFactorToBaseUnit;

        decimal converted = baseQuantity / to.ConversionFactorToBaseUnit;

        return Result.Success(converted);
    }
}