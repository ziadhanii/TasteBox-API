namespace TasteBox.Controllers;

[Authorize(Roles = DefaultRoles.Customer)]
[ApiExplorerSettings(GroupName = ApiDocuments.Mobile)]
public class CartController(ICartService cartService) : APIBaseController
{
    [HttpGet("")]
    [Cache(300)]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var result = await cartService.GetCartAsync(User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("items")]
    [InvalidateCache(CacheKeys.CartPattern)]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request, CancellationToken cancellationToken)
    {
        var result = await cartService.AddToCartAsync(User.GetUserId()!, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("items/{cartItemId}")]
    [InvalidateCache(CacheKeys.CartPattern)]
    public async Task<IActionResult> UpdateCartItem(
        [FromRoute] int cartItemId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await cartService.UpdateCartItemAsync(User.GetUserId()!, cartItemId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("items/{cartItemId}")]
    [InvalidateCache(CacheKeys.CartPattern)]
    public async Task<IActionResult> RemoveFromCart([FromRoute] int cartItemId, CancellationToken cancellationToken)
    {
        var result = await cartService.RemoveFromCartAsync(User.GetUserId()!, cartItemId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("clear")]
    [InvalidateCache(CacheKeys.CartPattern)]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var result = await cartService.ClearCartAsync(User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("items/count")]
    [Cache(300)]
    public async Task<IActionResult> GetCartItemsCount(CancellationToken cancellationToken)
    {
        var count = await cartService.GetCartItemsCountAsync(User.GetUserId()!, cancellationToken);
        return Ok(new { Count = count });
    }
}