namespace TasteBox.Contracts.Order;

public record CreateOrderRequest(
    PaymentMethod PaymentMethod,
    int? AddressId,
    string? Notes
);
