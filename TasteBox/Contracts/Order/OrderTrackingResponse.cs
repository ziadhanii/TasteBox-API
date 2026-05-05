namespace TasteBox.Contracts.Order;

public record OrderTrackingResponse(
    int OrderId,
    OrderStatus Status,
    IEnumerable<OrderStatusStep> Timeline
);