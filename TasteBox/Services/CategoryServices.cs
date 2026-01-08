using TasteBox.Abstractions;
using TasteBox.Errors;
using TasteBox.Utilities;
using TasteBox.Utilities.File;

namespace TasteBox.Services;

public class CategoryServices(ApplicationDbContext context, IFileService fileService) : ICategoryServices
{
    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Categories
            .AsNoTracking()
            .ProjectToType<CategoryResponse>()
            .ToListAsync(cancellationToken);

    public async Task<Result<CategoryResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await context.Categories.FindAsync(id, cancellationToken);

        return category is not null
            ? Result.Success(category.Adapt<CategoryResponse>())
            : Result.Failure<CategoryResponse>(CategoryErrors.CategoryNotFound);
    }

    public async Task<Result<CategoryResponse>> AddAsync(CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var isExistingName =
            await context.Categories.AnyAsync(x => x.Name == request.Name, cancellationToken: cancellationToken);

        if (isExistingName)
            return Result.Failure<CategoryResponse>(CategoryErrors.DuplicatedCategoryTitle);

        var category = request.Adapt<Category>();

        try
        {
            category.ImageUrl = await fileService.UploadAsync(
                request.ImageFile,
                nameof(UploadDirectory.Categories),
                cancellationToken);
        }
        catch
        {
            category.ImageUrl = "";
        }


        /* if (request.ImageFile is not null && string.IsNullOrEmpty(category.ImageUrl))
            return Result.Failure<CategoryResponse>(CommonErrors.FileUploadFailed);*/

        await context.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(category.Adapt<CategoryResponse>());
    }

    public async Task<Result> UpdateAsync(int id, UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null)
            return Result.Failure(CategoryErrors.CategoryNotFound);

        var isExistingName = await context.Categories
            .AnyAsync(x => x.Name == request.Name && x.Id != id, cancellationToken: cancellationToken);

        if (isExistingName)
            return Result.Failure(CategoryErrors.DuplicatedCategoryTitle);

        category.Name = request.Name ?? category.Name;

        try
        {
            if (request.ImageFile is not null)
            {
                category.ImageUrl = await fileService.ReplaceAsync(
                    request.ImageFile,
                    category.ImageUrl,
                    nameof(UploadDirectory.Categories),
                    cancellationToken);
            }
        }
        catch
        {
            // Ignore file upload errors
        }

        /* if (request.ImageFile is not null && string.IsNullOrEmpty(category.ImageUrl))
            return Result.Failure(CommonErrors.FileUploadFailed);*/

        context.Update(category);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    public async Task<Result> ToggleStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await context.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null)
            return Result.Failure(CategoryErrors.CategoryNotFound);

        category.IsDeleted = !category.IsDeleted;
        category.DeletedAt = category.IsDeleted ? DateTime.UtcNow : null;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}