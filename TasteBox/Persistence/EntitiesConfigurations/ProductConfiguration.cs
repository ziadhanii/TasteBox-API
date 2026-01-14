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

        builder.Property(c => c.ImageUrl)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<Category>()
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        builder.HasOne(p => p.Unit)
            .WithMany()
            .HasForeignKey(p => p.UnitId);

        builder.HasIndex(p => new { p.CategoryId, p.Name })
            .IsUnique()
            .HasFilter($"[{nameof(ISoftDelete.IsDeleted)}] = 0");

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}