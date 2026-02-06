namespace TasteBox.Contracts.Order;

public record OrderSummaryResponse(
    int Id,
    string OrderNumber,
    OrderStatus Status,
    PaymentMethod PaymentMethod,
    decimal TotalAmount,
    DateTime OrderDate,
    int ItemsCount
);
