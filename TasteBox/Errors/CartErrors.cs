using TasteBox.Abstractions;

namespace TasteBox.Errors;

public static class CartErrors
{
    public static readonly Error CartNotFound =
        new("Cart.NotFound", "No Cart was found for the given user", StatusCodes.Status404NotFound);

    public static readonly Error CartItemNotFound =
        new("CartItem.NotFound", "The specified item was not found in the cart", StatusCodes.Status404NotFound);

    public static readonly Error InvalidQuantity =
        new("Cart.InvalidQuantity", "The quantity must be greater than zero", StatusCodes.Status400BadRequest);

    public static readonly Error InsufficientStock =
        new("Cart.InsufficientStock", "The requested quantity exceeds available stock", StatusCodes.Status400BadRequest);

    public static readonly Error ProductNotAvailable =
        new("Cart.ProductNotAvailable", "The product is not available for purchase", StatusCodes.Status400BadRequest);

    public static readonly Error EmptyCart =
        new("Cart.Empty", "The cart is empty", StatusCodes.Status400BadRequest);

    public static readonly Error MaxOrderQuantityExceeded =
        new("Cart.MaxOrderQuantityExceeded", "The quantity exceeds the maximum order quantity for this product", StatusCodes.Status400BadRequest);

    public static readonly Error MinOrderQuantityNotMet =
        new("Cart.MinOrderQuantityNotMet", "The quantity is below the minimum order quantity for this product", StatusCodes.Status400BadRequest);
}
