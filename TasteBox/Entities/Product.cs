namespace TasteBox.Entities;

public class Product : ISoftDelete
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }


    // relationships
    public int CategoryId { get; set; }
    public int UnitId { get; set; }

    // navigation properties

    public Unit Unit { get; set; } = default!;

    // soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}