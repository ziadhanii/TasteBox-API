using TasteBox.Abstractions;
using TasteBox.Contracts.Product;
using TasteBox.Utilities;

namespace TasteBox.Interfaces;

public interface IProductService
{
    Task<PaginatedList<ProductResponse>> GetAllByCategoryIdAsync(
        int categoryId,
        RequestFilters filters,
        CancellationToken cancellationToken = default);

    Task<Result<ProductResponse>> GetByIdAsync(int categoryId, int id,
        CancellationToken cancellationToken = default);
}