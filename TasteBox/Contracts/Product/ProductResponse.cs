namespace TasteBox.Contracts.Product;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    string ImageUrl,
    decimal Quantity,
    int Stock,
    decimal Price,
    string Unit,
    string UnitSymbol
    );