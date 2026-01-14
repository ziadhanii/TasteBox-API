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

    Task<Result<ProductResponse>> CreateProductAsync(
        int categoryId,
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateProductAsync(
        int categoryId,
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);

    // TOGGLE STATUS SOFT DELETE
    Task<Result> ToggleProductStatusAsync(
        int categoryId,
        int id,
        CancellationToken cancellationToken = default);
}