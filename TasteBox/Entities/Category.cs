namespace TasteBox.Entities;

public class Category : ISoftDelete, IAuditable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    // relationships
    public ICollection<Product> Products { get; set; } = [];

    // audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}