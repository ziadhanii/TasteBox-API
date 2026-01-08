using TasteBox.Extensions;
using TasteBox.Utilities;

namespace TasteBox.Controllers;

[ApiController]
[Route("api/v1/category/{categoryId}/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAll(
        [FromRoute] int categoryId,
        [FromQuery] RequestFilters filters,
        CancellationToken cancellationToken)
    {
        var result = await productService.GetAllByCategoryIdAsync(categoryId, filters, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int categoryId, int id, CancellationToken cancellationToken)
    {
        var result = await productService.GetByIdAsync(categoryId, id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}