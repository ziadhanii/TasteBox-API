using TasteBox.Abstractions;

namespace TasteBox.Errors;

public class UnitErrors
{
    public static readonly Error UnitNotFound =
        new("Unit.NotFound", "No Unit was found with the given ID", StatusCodes.Status404NotFound);
}