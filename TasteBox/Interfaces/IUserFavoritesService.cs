using TasteBox.Abstractions;

namespace TasteBox.Interfaces;

public interface IUserFavoritesService
{
    Task<IEnumerable<ProductResponse>> GetUserFavoritesAsync(string userId, CancellationToken cancellationToken);
    Task<Result> AddToFavoritesAsync(string userId, int productId, CancellationToken cancellationToken);

    Task<Result> RemoveFromFavoritesAsync(string userId, int productId, CancellationToken cancellationToken);

    Task<Result> RemoveAllFavoritesAsync(string userId, CancellationToken cancellationToken);

    Task<bool> IsFavoriteAsync(string userId, int productId, CancellationToken cancellationToken);

    Task<int> GetFavoritesCountAsync(string userId, CancellationToken cancellationToken);
}