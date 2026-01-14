namespace TasteBox.Persistence.EntitiesConfigurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {

        builder
            .HasOne(i => i.Product)
            .WithOne(p => p.Stock)
            .HasForeignKey<Stock>(i => i.ProductId);
    }
}