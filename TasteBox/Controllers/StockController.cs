namespace TasteBox.Controllers;

[Route("api/v1/products/{productId}/stock")]
[ApiExplorerSettings(GroupName = APIDocuments.Dashboard)]
public class StockController(IStockService stockService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStock([FromRoute] int productId)
    {
        var result = await stockService.GetStockByProductIdAsync(productId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("increase")]
    public async Task<IActionResult> IncreaseQuantity(
        [FromRoute] int productId,
        AddQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await stockService.AddQuantityAsync(productId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPost("decrease")]
    public async Task<IActionResult> DecreaseQuantity(
        [FromRoute] int productId,
        RemoveQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await stockService.DeductQuantityAsync(productId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("/api/v1/stock/low")]
    public async Task<IActionResult> GetLowStockProducts(CancellationToken cancellationToken)
    {
        var result = await stockService.GetLowStockProductsAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}