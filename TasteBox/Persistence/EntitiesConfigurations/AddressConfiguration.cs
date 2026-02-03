namespace TasteBox.Persistence.EntitiesConfigurations;

public class AddressConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsMany(u => u.Addresses, addressBuilder =>
        {
            addressBuilder.ToTable("Addresses");

            addressBuilder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(200);

            addressBuilder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            addressBuilder.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(100);

            addressBuilder.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(20);

            addressBuilder.Property(a => a.BuildingNumber)
                .HasMaxLength(50);

            addressBuilder.Property(a => a.Floor)
                .HasMaxLength(20);

            addressBuilder.Property(a => a.Apartment)
                .HasMaxLength(20);

            addressBuilder.Property(a => a.Landmark)
                .HasMaxLength(200);

            addressBuilder.Property(a => a.PhoneNumber)
                .HasMaxLength(20);
        });
    }
}
