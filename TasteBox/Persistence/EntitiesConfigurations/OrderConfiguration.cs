namespace TasteBox.Persistence.EntitiesConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.PaymentMethod)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.SubTotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.ShippingCost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.Tax)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.Discount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.ShippingCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.ShippingState)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.ShippingZipCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.ShippingPhone)
            .HasMaxLength(20);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.OrderDate);
    }
}
