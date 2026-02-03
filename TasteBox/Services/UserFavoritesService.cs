using TasteBox.Abstractions;

namespace TasteBox.Services;

public class UserFavoritesService(ApplicationDbContext context) : IUserFavoritesService
{
    public async Task<IEnumerable<ProductResponse>> GetUserFavoritesAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        return await context.UserFavorites
            .AsNoTracking()
            .Where(uf => uf.UserId == userId)
            .Select(uf => uf.Product)
            .Where(p => !p.IsDeleted)
            .ProjectToType<ProductResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<Result> AddToFavoritesAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken)
    {
        var productExists = await context.Products
            .AsNoTracking()
            .AnyAsync(p => p.Id == productId && !p.IsDeleted, cancellationToken);

        if (!productExists)
            return Result.Failure(ProductErrors.ProductNotFound);

        var alreadyFavorited = await context
            .UserFavorites
            .AsNoTracking()
            .AnyAsync(uf => uf.UserId == userId && uf.ProductId == productId, cancellationToken);

        if (alreadyFavorited)
            return Result.Failure(UserFavoritesErrors.UserFavoriteAlreadyExists);

        var userFavorite = new UserFavorite
        {
            UserId = userId,
            ProductId = productId
        };

        context.UserFavorites.Add(userFavorite);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveFromFavoritesAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken)
    {
        var deleted = await context.UserFavorites
            .Where(uf => uf.UserId == userId && uf.ProductId == productId)
            .ExecuteDeleteAsync(cancellationToken);

        return deleted == 0 ? Result.Failure(UserFavoritesErrors.UserFavoriteNotFound) : Result.Success();
    }

    public async Task<Result> RemoveAllFavoritesAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        await context.UserFavorites
            .Where(uf => uf.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public Task<bool> IsFavoriteAsync(string userId, int productId, CancellationToken cancellationToken)
        => context.UserFavorites
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.ProductId == productId, cancellationToken);

    public async Task<int> GetFavoritesCountAsync(string userId, CancellationToken cancellationToken)
    {
        return await context.UserFavorites
            .AsNoTracking()
            .CountAsync(uf => uf.UserId == userId, cancellationToken);
    }
}