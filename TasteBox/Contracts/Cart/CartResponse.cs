namespace TasteBox.Contracts.Cart;

public record CartResponse(
    int Id,
    string UserId,
    int ItemsCount,
    IEnumerable<CartItemResponse> Items,
    decimal TotalAmount
);