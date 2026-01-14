namespace TasteBox.Persistence.EntitiesConfigurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Symbol)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(u => u.Name)
            .IsUnique();

        builder.HasIndex(u => u.Symbol)
            .IsUnique();
    }
}