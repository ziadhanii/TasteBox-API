using TasteBox.Abstractions;
using TasteBox.Contracts.Product;
using TasteBox.Errors;
using TasteBox.Utilities;
using System.Linq.Dynamic.Core;


namespace TasteBox.Services;

public class ProductService(ApplicationDbContext context) : IProductService
{
    public async Task<PaginatedList<ProductResponse>> GetAllByCategoryIdAsync(
        int categoryId,
        RequestFilters filters,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId);


        if (!string.IsNullOrWhiteSpace(filters.SearchValue))
            query = query.Where(p => p.Name.Contains(filters.SearchValue));


        if (!string.IsNullOrWhiteSpace(filters.SortColumn))
            query = query.OrderBy($"{filters.SortColumn} {filters.SortDirection}");

        else
            query = query.OrderBy(p => p.Id);


        var source = query.ProjectToType<ProductResponse>();

        var products = await PaginatedList<ProductResponse>.CreateAsync(
            source,
            filters.PageNumber,
            filters.PageSize,
            cancellationToken);

        return products;
    }


    public async Task<Result<ProductResponse>> GetByIdAsync(int categoryId, int id,
        CancellationToken cancellationToken = default)
    {
        var categoryExists = await context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == categoryId, cancellationToken);

        if (!categoryExists)
            return Result.Failure<ProductResponse>(CategoryErrors.CategoryNotFound);

        var product = await context.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId && p.Id == id)
            .ProjectToType<ProductResponse>()
            .FirstOrDefaultAsync(cancellationToken);

        return product is not null
            ? Result.Success(product)
            : Result.Failure<ProductResponse>(ProductErrors.ProductNotFound);
    }
}