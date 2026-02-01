using TasteBox.Abstractions;

namespace TasteBox.Helpers;

public interface IUnitConverter
{
    public Result<decimal> Convert(decimal quantity, Unit from, Unit to);

    decimal ToBase(decimal quantity, Unit unit);
    bool CanConvert(Unit from, Unit to);
}