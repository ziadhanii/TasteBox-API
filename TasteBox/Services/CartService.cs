namespace TasteBox.Services;

public class CartService(ApplicationDbContext context) : ICartService
{
    public async Task<Result<CartResponse>> GetCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await context.Carts
            .Where(c => c.UserId == userId)
            .ProjectToType<CartResponse>()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is not null)
            return Result.Success(cart);

        return Result.Success(EmptyCart(userId));
    }

    public async Task<Result<CartResponse>> AddToCartAsync(
        string userId,
        AddToCartRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
            return Result.Failure<CartResponse>(CartErrors.InvalidQuantity);

        var product = await context.Products
            .Include(p => p.Stock)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, cancellationToken);

        if (product is null)
            return Result.Failure<CartResponse>(ProductErrors.ProductNotFound);

        if (request.Quantity < product.MinOrderQty)
            return Result.Failure<CartResponse>(CartErrors.MinOrderQuantityNotMet);

        if (request.Quantity > product.MaxOrderQty)
            return Result.Failure<CartResponse>(CartErrors.MaxOrderQuantityExceeded);

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            context.Carts.Add(cart);
        }

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);

        var totalQuantity = existingItem is not null
            ? existingItem.Quantity + request.Quantity
            : request.Quantity;

        // ✅ FIX: stock check على الكمية الكلية مش request بس
        if (product.Stock.Quantity < totalQuantity)
            return Result.Failure<CartResponse>(CartErrors.InsufficientStock);

        if (existingItem is not null)
        {
            existingItem.Quantity = totalQuantity;
            existingItem.Price = product.DiscountedPrice ?? product.UnitPrice;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = product.DiscountedPrice ?? product.UnitPrice,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync(cancellationToken);

        return await GetCartAsync(userId, cancellationToken);
    }

    public async Task<Result<CartResponse>> UpdateCartItemAsync(
        string userId,
        int cartItemId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
            return Result.Failure<CartResponse>(CartErrors.InvalidQuantity);

        var item = await context.CartItems
            .Include(ci => ci.Product)
            .ThenInclude(p => p.Stock)
            .Include(ci => ci.Cart) // ✅ مهم عشان الأمان
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId, cancellationToken);

        if (item is null)
            return Result.Failure<CartResponse>(CartErrors.CartItemNotFound);

        var product = item.Product;

        if (request.Quantity < product.MinOrderQty)
            return Result.Failure<CartResponse>(CartErrors.MinOrderQuantityNotMet);

        if (request.Quantity > product.MaxOrderQty)
            return Result.Failure<CartResponse>(CartErrors.MaxOrderQuantityExceeded);

        if (product.Stock.Quantity < request.Quantity)
            return Result.Failure<CartResponse>(CartErrors.InsufficientStock);

        item.Quantity = request.Quantity;
        item.Price = product.DiscountedPrice ?? product.UnitPrice;

        await context.SaveChangesAsync(cancellationToken);

        return await GetCartAsync(userId, cancellationToken);
    }

    public async Task<Result> RemoveFromCartAsync(
        string userId,
        int cartItemId,
        CancellationToken cancellationToken = default)
    {
        var deleted = await context.CartItems
            .Where(ci => ci.Id == cartItemId && ci.Cart.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted == 0)
            return Result.Failure(CartErrors.CartItemNotFound);

        return Result.Success();
    }

    public async Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var deleted = await context.CartItems
            .Where(ci => ci.Cart.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<int> GetCartItemsCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.Cart.UserId == userId)
            .CountAsync(cancellationToken);
    }

    private static CartResponse EmptyCart(string userId)
        => new(0, userId, 0, [], 0);
}