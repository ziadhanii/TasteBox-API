namespace TasteBox.Persistence.EntitiesConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.PricePerBaseQuantity)
            .HasPrecision(18, 2);

        builder.Property(p => p.BaseQuantity)
            .HasPrecision(18, 3);

        builder.HasOne<Category>()
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        builder.HasOne(p => p.Unit)
            .WithMany()
            .HasForeignKey(p => p.UnitId);

        // Unique product name within a category (for non-deleted products)
        builder.HasIndex(p => new { p.CategoryId, p.Name })
            .IsUnique()
            .HasFilter($"[{nameof(ISoftDelete.IsDeleted)}] = 0");

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}