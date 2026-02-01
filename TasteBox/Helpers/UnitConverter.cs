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

        decimal baseQuantity = from.ConversionFactorToBaseUnit == 1m
            ? quantity
            : quantity * from.ConversionFactorToBaseUnit;

        decimal converted = to.ConversionFactorToBaseUnit == 1m
            ? baseQuantity
            : baseQuantity / to.ConversionFactorToBaseUnit;

        return Result.Success(converted);
    }
}