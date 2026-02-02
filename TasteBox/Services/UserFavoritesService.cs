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
            .ProjectToType<ProductResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<Result> AddToFavoritesAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken)
    {
        context.UserFavorites.Add(new UserFavorite
        {
            UserId = userId,
            ProductId = productId
        });

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch
        {
            return Result.Failure(UserFavoritesErrors.UserFavoriteAlreadyExists);
        }
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


    public Task<int> GetFavoritesCountAsync(string userId, CancellationToken cancellationToken)
        => context.UserFavorites
            .AsNoTracking()
            .CountAsync(x => x.UserId == userId, cancellationToken);
}