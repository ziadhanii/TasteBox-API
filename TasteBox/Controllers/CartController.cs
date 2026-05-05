namespace TasteBox.Controllers;

[Authorize(Roles = DefaultRoles.Customer)]
[ApiExplorerSettings(GroupName = APIDocuments.Mobile)]
public class CartController(ICartService cartService) : APIBaseController
{
    [Cache(5, CacheKeys.Cart)]
    [HttpGet("")]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var result = await cartService.GetCartAsync(User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [InvalidateCache(CacheKeys.Cart, CacheKeys.CartItemsCount)]
    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request, CancellationToken cancellationToken)
    {
        var result = await cartService.AddToCartAsync(User.GetUserId()!, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [InvalidateCache(CacheKeys.Cart, CacheKeys.CartItemsCount)]
    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItem(
        [FromRoute] int cartItemId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await cartService.UpdateCartItemAsync(User.GetUserId()!, cartItemId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [InvalidateCache(CacheKeys.Cart, CacheKeys.CartItemsCount)]
    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart([FromRoute] int cartItemId, CancellationToken cancellationToken)
    {
        var result = await cartService.RemoveFromCartAsync(User.GetUserId()!, cartItemId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [InvalidateCache(CacheKeys.Cart, CacheKeys.CartItemsCount)]
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var result = await cartService.ClearCartAsync(User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [Cache(5, CacheKeys.CartItemsCount)]
    [HttpGet("items/count")]
    public async Task<IActionResult> GetCartItemsCount(CancellationToken cancellationToken)
    {
        var count = await cartService.GetCartItemsCountAsync(User.GetUserId()!, cancellationToken);
        return Ok(new { Count = count });
    }
}
