namespace TasteBox.Contracts.Category;

public record CreateCategoryRequest(
    string Name,
    IFormFile ImageFile
);