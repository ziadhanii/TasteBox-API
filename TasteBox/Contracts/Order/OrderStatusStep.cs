namespace TasteBox.Contracts.Order;

public record OrderStatusStep(
    OrderStatus Status,
    DateTime? Time,
    bool IsCompleted
);