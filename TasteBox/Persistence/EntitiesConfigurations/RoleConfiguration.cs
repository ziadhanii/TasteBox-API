namespace TasteBox.Persistence.EntitiesConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData([
            new ApplicationRole
            {
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper(),
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
                CreatedById = DefaultUsers.AdminId,
                CreatedOn = DateTime.UtcNow
            },
            new ApplicationRole
            {
                Id = DefaultRoles.CustomerRoleId,
                Name = DefaultRoles.Customer,
                NormalizedName = DefaultRoles.Customer.ToUpper(),
                ConcurrencyStamp = DefaultRoles.CustomerRoleConcurrencyStamp,
                IsDefault = true,
                CreatedById = DefaultUsers.AdminId,
                CreatedOn = DateTime.UtcNow
            }
        ]);
    }
}