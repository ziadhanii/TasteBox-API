using TasteBox.Abstractions;

namespace TasteBox.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<CategoryWithProductsResponse>> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<CategoryResponse>> AddAsync(CreateCategoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default);
}