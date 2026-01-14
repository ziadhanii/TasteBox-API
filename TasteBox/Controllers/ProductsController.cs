using TasteBox.Contracts.Product;
using TasteBox.Extensions;
using TasteBox.Utilities;

namespace TasteBox.Controllers;

[ApiController]
[Route("api/v1/categories/{categoryId}/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet("")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Mobile)]
    public async Task<IActionResult> GetAll(
        [FromRoute] int categoryId,
        [FromQuery] RequestFilters filters,
        CancellationToken cancellationToken)
    {
        var result = await productService.GetAllByCategoryIdAsync(categoryId, filters, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Mobile)]
    public async Task<IActionResult> GetById(int categoryId, int id, CancellationToken cancellationToken)
    {
        var result = await productService.GetByIdAsync(categoryId, id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    [ApiExplorerSettings(GroupName = ApiDocuments.Dashboard)]
    public async Task<IActionResult> CreateProduct([FromRoute] int categoryId, [FromForm] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await productService.CreateProductAsync(categoryId, request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { categoryId = categoryId, id = result.Value!.Id }, null)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Dashboard)]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute] int categoryId,
        [FromRoute] int id,
        [FromForm] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await productService.UpdateProductAsync(categoryId, id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{id}/toggle-status")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Dashboard)]
    public async Task<IActionResult> ToggleProductStatus(
        [FromRoute] int categoryId,
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await productService.ToggleProductStatusAsync(categoryId, id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}