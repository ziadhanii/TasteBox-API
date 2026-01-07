namespace TasteBox.Controllers;

public class CategoriesController(ICategoryServices categoryServices) : APIBaseController
{
    [HttpGet("")]
    public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
        => Ok(await categoryServices.GetAllAsync(cancellationToken));
}