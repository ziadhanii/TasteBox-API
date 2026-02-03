using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TasteBox.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IHttpContextAccessor httpContextAccessor) :
    IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public DbSet<Stock> Stock { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Unit> Units { get; set; }

    public DbSet<UserFavorite> UserFavorites { get; set; }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = httpContextAccessor.HttpContext?.User.GetUserId();

        // Handle AuditableEntity
        var auditableEntries = ChangeTracker.Entries<AuditableEntity>();
        foreach (var entityEntry in auditableEntries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId!;
                entityEntry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }

        // Handle ApplicationRole (has same audit properties but doesn't inherit from AuditableEntity)
        var roleEntries = ChangeTracker.Entries<ApplicationRole>();
        foreach (var entityEntry in roleEntries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId!;
                entityEntry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                entityEntry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }

        // Handle Soft Delete
        var softDeleteEntries = ChangeTracker.Entries<ISoftDelete>();
        foreach (var entityEntry in softDeleteEntries)
        {
            if (entityEntry.State == EntityState.Deleted)
            {
                entityEntry.State = EntityState.Modified;
                entityEntry.Property(nameof(ISoftDelete.IsDeleted)).CurrentValue = true;
                entityEntry.Property(nameof(ISoftDelete.DeletedAt)).CurrentValue = DateTime.UtcNow;
            }
        }

        // Handle ApplicationRole soft delete (doesn't implement ISoftDelete but has same properties)
        var roleDeleteEntries = ChangeTracker.Entries<ApplicationRole>()
            .Where(e => e.State == EntityState.Deleted);
        foreach (var entityEntry in roleDeleteEntries)
        {
            entityEntry.State = EntityState.Modified;
            entityEntry.Property(x => x.IsDeleted).CurrentValue = true;
            entityEntry.Property(x => x.DeletedAt).CurrentValue = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        // // Configure AuditableEntity relationships globally
        // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        // {
        //     if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
        //     {
        //         modelBuilder.Entity(entityType.ClrType)
        //             .HasOne(typeof(ApplicationUser), nameof(AuditableEntity.CreatedBy))
        //             .WithMany()
        //             .HasForeignKey(nameof(AuditableEntity.CreatedById))
        //             .OnDelete(DeleteBehavior.Restrict);
        //
        //         modelBuilder.Entity(entityType.ClrType)
        //             .HasOne(typeof(ApplicationUser), nameof(AuditableEntity.UpdatedBy))
        //             .WithMany()
        //             .HasForeignKey(nameof(AuditableEntity.UpdatedById))
        //             .OnDelete(DeleteBehavior.Restrict);
        //     }
        // }

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var decimalProperties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));
            foreach (var property in decimalProperties)
            {
                modelBuilder.Entity(entityType.Name).Property(property.Name)
                    .HasPrecision(18, 3);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}