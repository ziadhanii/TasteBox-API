using TasteBox.Abstractions;

namespace TasteBox.Errors;

public class UserFavoritesErrors
{
    public static readonly Error UserFavoriteNotFound =
        new("UserFavorites.NotFound", "The product is not in the user's favorites.", StatusCodes.Status404NotFound);

    public static readonly Error UserFavoriteAlreadyExists =
        new("UserFavorites.AlreadyExists", "The product is already in the user's favorites.",
            StatusCodes.Status409Conflict);
}