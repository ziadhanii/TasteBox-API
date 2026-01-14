using TasteBox.Abstractions;

namespace TasteBox.Errors;

public static class ProductErrors
{
    public static readonly Error ProductNotFound =
        new("Product.NotFound", "No Product was found with the given ID", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedProductTitle =
        new("Product.DuplicatedTitle", "A Product with the given title already exists",
            StatusCodes.Status409Conflict);

}