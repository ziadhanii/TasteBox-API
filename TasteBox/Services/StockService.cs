using TasteBox.Abstractions;
using TasteBox.Contracts.Stock;
using TasteBox.Errors;

namespace TasteBox.Services;

public class StockService(ApplicationDbContext context) : IStockService
{
    public async Task<Result> AddQuantityAsync(int productId, AddQuantityRequest request,
        CancellationToken cancellationToken = default)
    {
        var product =
            await context
                .Products
                .Include(product => product.Stock)
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.ProductNotFound);

        product.Stock.Quantity += request.QuantityToAdd;
        product.Stock.LastUpdated = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveQuantityAsync(int productId, RemoveQuantityRequest request,
        CancellationToken cancellationToken = default)
    {
        var product =
            await context
                .Products
                .Include(product => product.Stock)
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.ProductNotFound);

        if (product.Stock.Quantity < request.QuantityToRemove)
            return Result.Failure(StockErrors.InsufficientStock);

        product.Stock.Quantity -= request.QuantityToRemove;
        product.Stock.LastUpdated = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<StockResponse>> GetStockByProductIdAsync(int productId,
        CancellationToken cancellationToken = default)
    {
        var stock = await context
            .Stock
            .AsNoTracking()
            .Where(s => s.ProductId == productId)
            .ProjectToType<StockResponse>()
            .FirstOrDefaultAsync(cancellationToken);

        return stock is null
            ? Result.Failure<StockResponse>(ProductErrors.ProductNotFound)
            : Result.Success(stock);
    }
}