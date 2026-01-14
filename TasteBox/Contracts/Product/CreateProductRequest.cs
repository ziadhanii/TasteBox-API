namespace TasteBox.Contracts.Product;

public record CreateProductRequest(
    string Name,
    string Description,
    IFormFile ImageFile,
    bool IsWeighedProduct,
    decimal InitialQuantity,
    decimal MinQuantityForStockAlerts,
    decimal CostPrice,
    decimal UnitPrice,
    decimal? DiscountedPrice,
    decimal MinOrderQty,
    decimal MaxOrderQty,
    int UnitId
);