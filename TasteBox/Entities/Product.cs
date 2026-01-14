using System.ComponentModel.DataAnnotations.Schema;

namespace TasteBox.Entities;

public class Product : ISoftDelete, IAuditable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public ProductType ProductType { get; set; }

    public decimal BaseQuantity { get; set; }
    public decimal? PricePerBaseQuantity { get; set; }

    public decimal? QuantityInStock { get; set; }


    [NotMapped]
    public bool HasStock => ProductType switch
    {
        ProductType.Simple => true,

        ProductType.Weighted => QuantityInStock > 0,

        ProductType.Variant => Variants != null && Variants.Any(v => v.QuantityInStock > 0),

        _ => false
    };

    // relations
    public int CategoryId { get; set; }

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = default!;

    public ICollection<ProductQuantityOption>? QuantityOptions { get; set; } = [];

    public ICollection<ProductVariant>? Variants { get; set; } = [];

    // audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}