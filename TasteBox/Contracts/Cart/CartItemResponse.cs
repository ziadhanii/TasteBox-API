namespace TasteBox.Contracts.Cart;

public record CartItemResponse(
    int Id,
    int ProductId,
    string ProductName,
    string ProductImage,
    decimal Quantity,
    decimal UnitPrice,
    decimal? DiscountedPrice,
    decimal Price,
    decimal Subtotal,
    string UnitName,
    string UnitSymbol,
    bool IsWeighedProduct,
    decimal AvailableStock,
    decimal MinOrderQty,
    decimal MaxOrderQty
);
