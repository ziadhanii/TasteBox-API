namespace TasteBox.Persistence.EntitiesConfigurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(i => i.Id);

        builder
            .HasOne(i => i.Product)
            .WithOne(p => p.Stock)
            .HasForeignKey<Stock>(i => i.ProductId);
    }
}