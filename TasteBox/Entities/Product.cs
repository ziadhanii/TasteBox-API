namespace TasteBox.Entities;

public class Product : ISoftDelete
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsWeighedProduct { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }

    public decimal? DiscountedPrice { get; set; }

    public decimal MaxOrderQty { get; set; }
    public decimal MinOrderQty { get; set; }
    public int CategoryId { get; set; }
    public int UnitId { get; set; }
    public Stock Stock { get; set; } = default!;
    public Unit Unit { get; set; } = default!;

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}