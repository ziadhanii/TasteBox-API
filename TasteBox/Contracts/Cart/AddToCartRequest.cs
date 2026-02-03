namespace TasteBox.Contracts.Cart;

public record AddToCartRequest(
    int ProductId,
    decimal Quantity
);
