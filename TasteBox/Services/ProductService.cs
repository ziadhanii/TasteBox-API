using TasteBox.Abstractions;
using TasteBox.Contracts.Product;
using TasteBox.Errors;
using TasteBox.Utilities;
using System.Linq.Dynamic.Core;
using TasteBox.Utilities.File;

namespace TasteBox.Services;

public class ProductService(ApplicationDbContext context, IFileService fileService) : IProductService
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

    public async Task<Result<ProductResponse>> GetByIdAsync(
        int categoryId,
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await context.Products
            .Where(p => p.Id == id && p.CategoryId == categoryId)
            .ProjectToType<ProductResponse>()
            .FirstOrDefaultAsync(cancellationToken);

        return product is null
            ? Result.Failure<ProductResponse>(ProductErrors.ProductNotFound)
            : Result.Success(product);
    }

    public async Task<Result<ProductResponse>> CreateProductAsync(
        int categoryId,
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var categoryExists = await context.Categories
            .AnyAsync(x => x.Id == categoryId, cancellationToken);

        if (!categoryExists)
            return Result.Failure<ProductResponse>(CategoryErrors.CategoryNotFound);

        var unitExists = await context.Units
            .AnyAsync(x => x.Id == request.UnitId, cancellationToken);

        if (!unitExists)
            return Result.Failure<ProductResponse>(UnitErrors.UnitNotFound);

        var product = new Product()
        {
            CategoryId = categoryId,
            UnitId = request.UnitId,
            Name = request.Name,
            Description = request.Description,
            IsWeighedProduct = request.IsWeighedProduct,
            UnitPrice = request.UnitPrice,
            CostPrice = request.CostPrice,
            DiscountedPrice = request.DiscountedPrice,
            MaxOrderQty = request.MaxOrderQty,
            MinOrderQty = request.MinOrderQty,
            ImageUrl = await fileService.UploadAsync(
                request.ImageFile,
                nameof(UploadDirectory.Products),
                cancellationToken)
        };

        context.Add(product);

        await context.SaveChangesAsync(cancellationToken);

        var stock = new Stock
        {
            ProductId = product.Id,
            Quantity = request.InitialQuantity,
            MinQuantity = request.MinQuantityForStockAlerts,
            LastUpdated = DateTime.UtcNow
        };

        context.Add(stock);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Adapt<ProductResponse>());
    }

    public async Task<Result> UpdateProductAsync(
        int categoryId,
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.CategoryId == categoryId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.ProductNotFound);

        if (request.UnitId.HasValue)
        {
            var unitExists = await context.Units
                .AnyAsync(x => x.Id == request.UnitId.Value, cancellationToken);

            if (!unitExists)
                return Result.Failure(UnitErrors.UnitNotFound);
        }

        request.Adapt(product);

        if (request.ImageFile is not null)
        {
            product.ImageUrl = await fileService.UploadAsync(
                request.ImageFile,
                nameof(UploadDirectory.Products),
                cancellationToken);
        }

        // product.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ToggleProductStatusAsync(int categoryId, int id,
        CancellationToken cancellationToken = default)
    {
        var product = await context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && p.CategoryId == categoryId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.ProductNotFound);
        product.IsDeleted = !product.IsDeleted;
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}