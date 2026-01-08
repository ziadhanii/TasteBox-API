namespace TasteBox.Contracts.Category;

public record UpdateCategoryRequest(
    string? Name,
    IFormFile? ImageFile
);