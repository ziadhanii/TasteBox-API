namespace TasteBox.Contracts.Product;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    string ImageUrl,
    decimal UnitPrice,
    decimal? DiscountedPrice,
    bool HasDiscount,
    bool IsWeighedProduct,
    string UnitName,
    string UnitSymbol,
    decimal MinOrderQty,
    decimal MaxOrderQty,
    decimal QuantityInStock,
    bool IsAvailable
);