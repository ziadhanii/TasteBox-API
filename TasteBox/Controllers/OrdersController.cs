namespace TasteBox.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/orders")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet("payment-method")]
    [ApiExplorerSettings(GroupName = APIDocuments.Mobile)]
    public IActionResult GetPaymentMethods()
    {
        var paymentMethods = Enum.GetValues<PaymentMethod>()
            .Select(x => new
            {
                Id = (int)x,
                Name = x.ToString()
            });
        return Ok(paymentMethods);
    }

    [HttpPost]
    [InvalidateCache(CacheKeys.Cart, CacheKeys.CartItemsCount)]
    [ApiExplorerSettings(GroupName = APIDocuments.Mobile)]
    [Authorize(Roles = DefaultRoles.Customer)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await orderService.CreateOrderAsync(User.GetUserId()!, request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOrder), new { id = result.Value!.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpGet("{id}")]
    [ApiExplorerSettings(GroupName = APIDocuments.Mobile)]
    [Authorize(Roles = DefaultRoles.Customer)]
    public async Task<IActionResult> GetOrder([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await orderService.GetOrderByIdAsync(User.GetUserId()!, id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    [ApiExplorerSettings(GroupName = APIDocuments.Mobile)]
    [Authorize(Roles = DefaultRoles.Customer)]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var result = await orderService.GetUserOrdersAsync(User.GetUserId()!, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}/cancel")]
    [ApiExplorerSettings(GroupName = APIDocuments.Mobile)]
    [Authorize(Roles = DefaultRoles.Customer)]
    public async Task<IActionResult> CancelOrder([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await orderService.CancelOrderAsync(User.GetUserId()!, id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("all")]
    [ApiExplorerSettings(GroupName = APIDocuments.Dashboard)]
    [HasPermission(Permissions.GetOrders)]
    public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
    {
        var result = await orderService.GetAllOrdersAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}/status")]
    [ApiExplorerSettings(GroupName = APIDocuments.Dashboard)]
    [HasPermission(Permissions.UpdateOrders)]
    public async Task<IActionResult> UpdateOrderStatus(
        [FromRoute] int id,
        [FromBody] UpdateOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await orderService.UpdateOrderStatusAsync(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


    [HttpGet("{orderId}/tracking")]
    public async Task<IActionResult> GetOrderTracking(int orderId, CancellationToken cancellationToken)
    {
        var result = await orderService.GetOrderTrackingAsync(User.GetUserId()!, orderId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
}