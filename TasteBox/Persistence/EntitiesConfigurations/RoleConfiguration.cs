using TasteBox.Abstractions.Consts;

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
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp
            },
            new ApplicationRole
            {
                Id = DefaultRoles.CustomerRoleId,
                Name = DefaultRoles.Customer,
                NormalizedName = DefaultRoles.Customer.ToUpper(),
                ConcurrencyStamp = DefaultRoles.CustomerRoleConcurrencyStamp,
                IsDefault = true
            }
        ]);
    }
}