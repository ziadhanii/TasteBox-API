using TasteBox.Extensions;
using TasteBox.Utilities;

namespace TasteBox.Controllers;

public class CategoriesController(ICategoryService categoryService) : APIBaseController
{
    [HttpGet("")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Mobile)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await categoryService.GetAllAsync(cancellationToken));


    [HttpGet("{id}")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Mobile)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await categoryService.GetAsync(id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Dashboard)]
    public async Task<IActionResult> Add([FromForm] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await categoryService.AddAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Dashboard)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await categoryService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{id}/toggleStatus")]
    [ApiExplorerSettings(GroupName = ApiDocuments.Dashboard)]
    public async Task<IActionResult> ToggleStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await categoryService.ToggleStatusAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}