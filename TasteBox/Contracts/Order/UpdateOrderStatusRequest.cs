namespace TasteBox.Contracts.Order;

public record UpdateOrderStatusRequest(
    OrderStatus Status
);
