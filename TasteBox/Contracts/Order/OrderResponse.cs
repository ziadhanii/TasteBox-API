namespace TasteBox.Contracts.Order;

public record OrderResponse(
    int Id,
    string OrderNumber,
    string UserId,
    OrderStatus Status,
    PaymentMethod PaymentMethod,
    decimal SubTotal,
    decimal ShippingCost,
    decimal Tax,
    decimal Discount,
    decimal TotalAmount,
    string ShippingAddress,
    string ShippingCity,
    string ShippingState,
    string ShippingZipCode,
    string? ShippingPhone,
    string? Notes,
    DateTime OrderDate,
    IEnumerable<OrderItemResponse> OrderItems,
    IEnumerable<OrderStatusStep> Timeline
);