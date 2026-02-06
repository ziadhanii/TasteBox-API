namespace TasteBox.Contracts.Order;

public record OrderItemResponse(
    int Id,
    int ProductId,
    string ProductName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
