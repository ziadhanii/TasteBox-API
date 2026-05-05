
namespace TasteBox.Interfaces;

public interface ICartService
{
    Task<Result<CartResponse>> GetCartAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result<CartResponse>> AddToCartAsync(string userId, AddToCartRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<CartResponse>> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveFromCartAsync(string userId, int cartItemId, CancellationToken cancellationToken = default);
    Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> GetCartItemsCountAsync(string userId, CancellationToken cancellationToken = default);
}