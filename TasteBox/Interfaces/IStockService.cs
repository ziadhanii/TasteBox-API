using TasteBox.Abstractions;
using TasteBox.Contracts.Stock;

namespace TasteBox.Interfaces;

public interface IStockService
{
    Task<Result> AddQuantityAsync(
        int productId,
        AddQuantityRequest request,
        CancellationToken cancellationToken = default);


    Task<Result> RemoveQuantityAsync(
        int productId,
        RemoveQuantityRequest request,
        CancellationToken cancellationToken = default);


    Task<Result<StockResponse>> GetStockByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);
}