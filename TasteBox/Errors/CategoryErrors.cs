using TasteBox.Abstractions;

namespace TasteBox.Errors;

public static class CategoryErrors
{
    public static readonly Error CategoryNotFound =
        new("Category.NotFound", "No Category was found with the given Id", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedCategoryTitle =
        new("Category.DuplicatedTitle", "A Category with the given title already exists",
            StatusCodes.Status409Conflict);
}