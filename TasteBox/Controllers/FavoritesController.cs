namespace TasteBox.Controllers;

[Authorize(Roles = DefaultRoles.Customer)]
[ApiExplorerSettings(GroupName = ApiDocuments.Mobile)]
public class FavoritesController(IUserFavoritesService favoritesService) : APIBaseController
{
    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await favoritesService.GetUserFavoritesAsync(User.GetUserId()!, cancellationToken));
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> Add([FromRoute] int productId, CancellationToken cancellationToken)
    {
        var result = await favoritesService.AddToFavoritesAsync(User.GetUserId()!, productId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove([FromRoute] int productId, CancellationToken cancellationToken)
    {
        var result = await favoritesService.RemoveFromFavoritesAsync(User.GetUserId()!, productId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken)
    {
        var result = await favoritesService.RemoveAllFavoritesAsync(User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("count")]
    public async Task<IActionResult> Count(CancellationToken cancellationToken)
    {
        var count = await favoritesService.GetFavoritesCountAsync(User.GetUserId()!, cancellationToken);
        return Ok(new { Count = count });
    }
}