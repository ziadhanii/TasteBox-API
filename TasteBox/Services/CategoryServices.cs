namespace TasteBox.Services;

public class CategoryServices(ApplicationDbContext context) : ICategoryServices
{
    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Categories
            .AsNoTracking()
            .ProjectToType<CategoryResponse>()
            .ToListAsync(cancellationToken);
}