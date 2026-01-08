using TasteBox.Contracts.Product;

namespace TasteBox.Contracts.Category;

public record CategoryWithProductsResponse(
    int Id,
    string Name,
    string ImageUrl,
    IEnumerable<ProductResponse> Products
);