namespace TasteBox.Contracts.Product;

public record UpdateProductRequest(
    string? Name,
    string? Description,
    IFormFile? ImageFile,
    bool? IsWeighedProduct,
    decimal? CostPrice,
    decimal? UnitPrice,
    decimal? DiscountedPrice,
    decimal? MinOrderQty,
    decimal? MaxOrderQty,
    int? UnitId
);