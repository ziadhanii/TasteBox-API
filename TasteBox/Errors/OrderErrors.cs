namespace TasteBox.Errors;

public static class OrderErrors
{
    public static readonly Error OrderNotFound =
        new("Order.NotFound", "No order was found with the given ID", StatusCodes.Status404NotFound);

    public static readonly Error EmptyCart =
        new("Order.EmptyCart", "Cannot create order from an empty cart", StatusCodes.Status400BadRequest);

    public static readonly Error InsufficientStock =
        new("Order.InsufficientStock", "One or more items have insufficient stock", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidAddress =
        new("Order.InvalidAddress", "The specified address is invalid or does not belong to the user", StatusCodes.Status404NotFound);

    public static readonly Error NoDefaultAddress =
        new("Order.NoDefaultAddress", "No default address found. Please add an address or specify one", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidStatusTransition =
        new("Order.InvalidStatusTransition", "Cannot change order status from current state", StatusCodes.Status400BadRequest);

    public static readonly Error CannotCancelOrder =
        new("Order.CannotCancel", "Order cannot be cancelled in its current state", StatusCodes.Status400BadRequest);

    public static readonly Error UnauthorizedAccess =
        new("Order.UnauthorizedAccess", "You do not have permission to access this order", StatusCodes.Status403Forbidden);

    public static readonly Error OrderAlreadyCancelled =
        new("Order.AlreadyCancelled", "This order has already been cancelled", StatusCodes.Status400BadRequest);

    public static readonly Error OrderAlreadyCompleted =
        new("Order.AlreadyCompleted", "This order has already been completed", StatusCodes.Status400BadRequest);
}
