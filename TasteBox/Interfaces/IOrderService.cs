using TasteBox.Abstractions;
using TasteBox.Contracts.Order;

namespace TasteBox.Interfaces;

public interface IOrderService
{
    Task<Result<OrderResponse>> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderResponse>> GetOrderByIdAsync(string userId, int orderId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<OrderSummaryResponse>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<OrderSummaryResponse>>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<Result> CancelOrderAsync(string userId, int orderId, CancellationToken cancellationToken = default);
    Task<Result> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
}
