namespace TasteBox.Persistence.EntitiesConfigurations;

public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FcmToken).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.FcmToken).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}