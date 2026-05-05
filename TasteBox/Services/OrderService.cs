using TasteBox.Contracts.Notification;

namespace TasteBox.Services;

public class OrderService(
    ApplicationDbContext context,
    IOptions<OrderSettings> orderSettings,
    ILogger<OrderService> logger,
    INotificationSender notificationSender) : IOrderService
{
    private readonly OrderSettings _orderSettings = orderSettings.Value;

    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
    {
        { OrderStatus.Pending, [OrderStatus.Confirmed, OrderStatus.Cancelled] },
        { OrderStatus.Confirmed, [OrderStatus.Preparing, OrderStatus.Cancelled] },
        { OrderStatus.Preparing, [OrderStatus.OutForDelivery] },
        { OrderStatus.OutForDelivery, [OrderStatus.Delivered] },
        { OrderStatus.Delivered, [] },
        { OrderStatus.Cancelled, [] }
    };

    public async Task<Result<OrderResponse>> CreateOrderAsync(
        string userId,
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var cartResult = await GetUserCartAsync(userId, cancellationToken);
            if (cartResult.IsFailure)
                return Result.Failure<OrderResponse>(cartResult.Error);

            var cart = cartResult.Value!;

            var addressQuery = context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Addresses);

            var address = request.AddressId.HasValue
                ? await addressQuery.FirstOrDefaultAsync(a => a.Id == request.AddressId.Value, cancellationToken)
                : await addressQuery.FirstOrDefaultAsync(a => a.IsDefault, cancellationToken);

            if (address is null)
            {
                var errorMessage = request.AddressId.HasValue
                    ? OrderErrors.InvalidAddress
                    : OrderErrors.NoDefaultAddress;

                logger.LogWarning(
                    "Address not found for user {UserId}. AddressId: {AddressId}",
                    userId,
                    request.AddressId);

                return Result.Failure<OrderResponse>(errorMessage);
            }

            var stockValidation = ValidateStock(cart.CartItems);
            if (stockValidation.IsFailure)
            {
                logger.LogWarning("Insufficient stock for user {UserId} order", userId);
                return Result.Failure<OrderResponse>(stockValidation.Error);
            }

            var order = CreateOrderEntity(userId, cart, address, request);

            DeductStock(cart.CartItems);

            context.Orders.Add(order);
            context.CartItems.RemoveRange(cart.CartItems);

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Order {OrderNumber} created successfully for user {UserId}",
                order.OrderNumber,
                userId);

            return Result.Success(order.Adapt<OrderResponse>());
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Error creating order for user {UserId}", userId);
            throw;
        }
    }

    public async Task<Result<OrderTrackingResponse>> GetOrderTrackingAsync(
        string userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);

        if (order is null)
            return Result.Failure<OrderTrackingResponse>(OrderErrors.OrderNotFound);

        var response = new OrderTrackingResponse(
            order.Id,
            order.Status,
            order.GetTimeline()
        );

        return Result.Success(response);
    }

    public async Task<Result<OrderResponse>> GetOrderByIdAsync(
        string userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await context.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId && o.UserId == userId)
            .ProjectToType<OrderResponse>()
            .FirstOrDefaultAsync(cancellationToken);

        return order is null
            ? Result.Failure<OrderResponse>(OrderErrors.OrderNotFound)
            : Result.Success(order);
    }

    public async Task<Result<IEnumerable<OrderSummaryResponse>>> GetUserOrdersAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var orders = await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ProjectToType<OrderSummaryResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<OrderSummaryResponse>>(orders);
    }

    public async Task<Result<IEnumerable<OrderSummaryResponse>>> GetAllOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ProjectToType<OrderSummaryResponse>()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<OrderSummaryResponse>>(orders);
    }

    public async Task<Result> CancelOrderAsync(
        string userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Stock)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);

            if (order is null)
                return Result.Failure(OrderErrors.OrderNotFound);

            if (order.Status == OrderStatus.Cancelled)
                return Result.Failure(OrderErrors.OrderAlreadyCancelled);

            if (!order.CanBeCancelled())
                return Result.Failure(OrderErrors.CannotCancelOrder);

            if (!AllowedTransitions.TryGetValue(order.Status, out var allowedStatuses) ||
                !allowedStatuses.Contains(OrderStatus.Cancelled))
                return Result.Failure(OrderErrors.InvalidStatusTransition);

            foreach (var orderItem in order.OrderItems)
            {
                var stock = orderItem.Product.Stock;

                if (stock != null)
                {
                    stock.Quantity += orderItem.Quantity;
                    stock.LastUpdated = DateTime.UtcNow;
                }
            }

            var result = order.ChangeStatus(OrderStatus.Cancelled, AllowedTransitions);
            if (result.IsFailure)
                return result;

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Order {OrderNumber} cancelled successfully by user {UserId}",
                order.OrderNumber,
                userId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Error cancelling order {OrderId} for user {UserId}", orderId, userId);
            throw;
        }
    }

    public async Task<Result> UpdateOrderStatusAsync(
        int orderId,
        UpdateOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.OrderNotFound);

        var result = order.ChangeStatus(request.Status, AllowedTransitions);
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        await notificationSender.SendToUserAsync(order.UserId, new SendNotificationRequest
        {
            Title = "Order Update 📦",
            Body = $"Your order is now {order.Status}"
        });
        logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, request.Status);

        return Result.Success();
    }

    private async Task<Result<Cart>> GetUserCartAsync(string userId, CancellationToken cancellationToken)
    {
        var cart = await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Stock)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null || !cart.CartItems.Any())
            return Result.Failure<Cart>(OrderErrors.EmptyCart);

        return Result.Success(cart);
    }

    private static Result ValidateStock(ICollection<CartItem> cartItems)
    {
        return cartItems.Any(ci => ci.Product.Stock.Quantity < ci.Quantity)
            ? Result.Failure(OrderErrors.InsufficientStock)
            : Result.Success();
    }

    private Order CreateOrderEntity(string userId, Cart cart, Address address, CreateOrderRequest request)
    {
        var calculator = new OrderCalculator(_orderSettings.TaxRate, _orderSettings.DefaultShippingCost);
        var calculation = calculator.Calculate(cart.CartItems);

        var orderNumber =
            $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";

        return new Order
        {
            OrderNumber = orderNumber,
            UserId = userId,
            Status = OrderStatus.Pending,
            PaymentMethod = request.PaymentMethod,
            SubTotal = calculation.SubTotal,
            ShippingCost = calculation.ShippingCost,
            Tax = calculation.Tax,
            Discount = calculation.Discount,
            TotalAmount = calculation.TotalAmount,
            ShippingAddress = address.Street,
            ShippingCity = address.City,
            ShippingState = address.State,
            ShippingZipCode = address.ZipCode,
            ShippingPhone = address.PhoneNumber,
            Notes = request.Notes,
            OrderDate = DateTime.UtcNow,
            OrderItems = cart.CartItems.Select(ci => new OrderItem
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Quantity = ci.Quantity,
                UnitPrice = ci.Price,
                TotalPrice = ci.Quantity * ci.Price
            }).ToList()
        };
    }

    private static void DeductStock(ICollection<CartItem> cartItems)
    {
        foreach (var cartItem in cartItems)
        {
            var stock = cartItem.Product.Stock;

            stock.Quantity = Math.Max(0, stock.Quantity - cartItem.Quantity);
            stock.LastUpdated = DateTime.UtcNow;
        }
    }
}