using TasteBox.Abstractions;

namespace TasteBox.Errors;

public static class StockErrors
{
    public static readonly Error InsufficientStock =
        new("Stock.Insufficient", "The Stock quantity is insufficient for the requested operation",
            StatusCodes.Status400BadRequest);
}