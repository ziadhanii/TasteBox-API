namespace TasteBox.Controllers;

[Route("api/v1/products/{productId}/stock")]
[ApiExplorerSettings(GroupName = ApiDocuments.Dashboard)]
public class StockController(IStockService stockService) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetStock([FromRoute] int productId)
    {
        var result = await stockService.GetStockByProductIdAsync(productId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("add")]
    public async Task<IActionResult> AddQuantity(
        [FromRoute] int productId,
        AddQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await stockService.AddQuantityAsync(productId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("deduct")]
    public async Task<IActionResult> DeductQuantity(
        [FromRoute] int productId,
        RemoveQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await stockService.DeductQuantityAsync(productId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}