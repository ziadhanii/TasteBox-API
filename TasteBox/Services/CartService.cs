using TasteBox.Abstractions;

namespace TasteBox.Services;

public class CartService(ApplicationDbContext context) : ICartService
{
    public async Task<Result<CartResponse>> GetCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await context.Carts
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ProjectToType<CartResponse>()
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is not null)
            return Result.Success(cart);

        var newCart = new Cart
        {
            UserId = userId,
            CartItems = []
        };

        context.Carts.Add(newCart);
        await context.SaveChangesAsync(cancellationToken);

        cart = newCart.Adapt<CartResponse>();

        return Result.Success(cart);
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
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, cancellationToken);

        if (product is null)
            return Result.Failure<CartResponse>(ProductErrors.ProductNotFound);

        // Validate quantity constraints
        if (request.Quantity < product.MinOrderQty)
            return Result.Failure<CartResponse>(CartErrors.MinOrderQuantityNotMet);

        if (request.Quantity > product.MaxOrderQty)
            return Result.Failure<CartResponse>(CartErrors.MaxOrderQuantityExceeded);

        // Check stock availability
        if (product.Stock.Quantity < request.Quantity)
            return Result.Failure<CartResponse>(CartErrors.InsufficientStock);

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Stock)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId,
                CartItems = []
            };
            context.Carts.Add(cart);
        }

        // Check if product already exists in cart
        var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);

        if (existingCartItem is not null)
        {
            var newQuantity = existingCartItem.Quantity + request.Quantity;

            // Validate new quantity
            if (newQuantity > product.MaxOrderQty)
                return Result.Failure<CartResponse>(CartErrors.MaxOrderQuantityExceeded);

            if (product.Stock.Quantity < newQuantity)
                return Result.Failure<CartResponse>(CartErrors.InsufficientStock);

            existingCartItem.Quantity = newQuantity;
            existingCartItem.Price = product.DiscountedPrice ?? product.UnitPrice;
        }
        else
        {
            var cartItem = new CartItem
            {
                Cart = cart,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = product.DiscountedPrice ?? product.UnitPrice,
                CreatedAt = DateTime.UtcNow
            };
            cart.CartItems.Add(cartItem);
        }

        await context.SaveChangesAsync(cancellationToken);

        // Reload cart with fresh data for mapping
        var cartResponse = await context.Carts
            .AsNoTracking()
            .Where(c => c.Id == cart.Id)
            .ProjectToType<CartResponse>()
            .FirstAsync(cancellationToken);

        return Result.Success(cartResponse);
    }

    public async Task<Result<CartResponse>> UpdateCartItemAsync(
        string userId,
        int cartItemId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
            return Result.Failure<CartResponse>(CartErrors.InvalidQuantity);

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Stock)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null)
            return Result.Failure<CartResponse>(CartErrors.CartNotFound);

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        if (cartItem is null)
            return Result.Failure<CartResponse>(CartErrors.CartItemNotFound);

        var product = cartItem.Product;

        // Validate quantity constraints
        if (request.Quantity < product.MinOrderQty)
            return Result.Failure<CartResponse>(CartErrors.MinOrderQuantityNotMet);

        if (request.Quantity > product.MaxOrderQty)
            return Result.Failure<CartResponse>(CartErrors.MaxOrderQuantityExceeded);

        // Check stock availability
        if (product.Stock.Quantity < request.Quantity)
            return Result.Failure<CartResponse>(CartErrors.InsufficientStock);

        cartItem.Quantity = request.Quantity;
        cartItem.Price = product.DiscountedPrice ?? product.UnitPrice;

        await context.SaveChangesAsync(cancellationToken);

        // Reload cart with fresh data for mapping
        var cartResponse = await context.Carts
            .AsNoTracking()
            .Where(c => c.Id == cart.Id)
            .ProjectToType<CartResponse>()
            .FirstAsync(cancellationToken);

        return Result.Success(cartResponse);
    }

    public async Task<Result> RemoveFromCartAsync(
        string userId,
        int cartItemId,
        CancellationToken cancellationToken = default)
    {
        var deleted = await context.CartItems
            .Where(ci => ci.Id == cartItemId && ci.Cart.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted == 0
            ? Result.Failure(CartErrors.CartItemNotFound)
            : Result.Success();
    }

    public async Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        await context.CartItems
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
}