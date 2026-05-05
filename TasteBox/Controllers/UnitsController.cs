using TasteBox.Contracts.Unit;

namespace TasteBox.Controllers;

[ApiExplorerSettings(GroupName = APIDocuments.Dashboard)]
public class UnitsController(IUnitService unitService) : APIBaseController
{
    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await unitService.GetAllAsync(cancellationToken);
        return Ok(result.Value);
    }


    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType([FromRoute] UnitType type, CancellationToken cancellationToken)
    {
        var result = await unitService.GetByTypeAsync(type, cancellationToken);
        return Ok(result.Value);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await unitService.GetAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    [Route("types")]
    public IActionResult GetAllTypes()
    {
        return Ok(Enum
            .GetValues<UnitType>()
            .Select(x => new
            {
                Id = (int)x,
                Name = x.ToString().ToLower()
            }));
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] CreateUnitRequest request, CancellationToken cancellationToken)
    {
        var result = await unitService.AddAsync(request, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value)
            : result.ToProblem();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateUnitRequest request,
        CancellationToken cancellationToken)
    {
        var result = await unitService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


    [HttpPut("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatusAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await unitService.ToggleStatusAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}