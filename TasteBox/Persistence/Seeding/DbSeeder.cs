namespace TasteBox.Persistence.Seeding;

using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

public static class DbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        var dbUnits = await context.Units.ToListAsync();

        if (!dbUnits.Any())
            throw new Exception("Units must be seeded first.");

        // ================= USERS =================
        var users = new List<ApplicationUser>();

        if (!await context.Users.AnyAsync())
        {
            var faker = new Faker<ApplicationUser>()
                .RuleFor(u => u.Id, _ => Guid.NewGuid().ToString())
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.UserName, (f, u) => u.Email)
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
                .RuleFor(u => u.NormalizedUserName, (f, u) => u.Email.ToUpper())
                .RuleFor(u => u.EmailConfirmed, true);

            users = faker.Generate(5);

            foreach (var user in users)
                await userManager.CreateAsync(user, "P@ssw0rd!");
        }
        else
        {
            users = await context.Users.ToListAsync();
        }

        // ================= CATEGORIES =================
        List<Category> categories;

        if (!await context.Categories.AnyAsync())
        {
            categories = new Faker<Category>()
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0] + "-" + f.IndexFaker)
                .RuleFor(c => c.ImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(c => c.CreatedOn, DateTime.UtcNow)
                .Generate(5);

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }
        else
        {
            categories = await context.Categories.ToListAsync();
        }

        // ================= PRODUCTS =================
        List<Product> products;

        if (!await context.Products.AnyAsync())
        {
            products = new Faker<Product>()
                .RuleFor(p => p.Name, f => $"{f.Commerce.ProductName()}-{f.IndexFaker}")
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.ImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(p => p.UnitPrice, f => decimal.Parse(f.Commerce.Price(), CultureInfo.InvariantCulture))
                .RuleFor(p => p.CostPrice, (f, p) => p.UnitPrice * 0.7m)
                .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id)
                .RuleFor(p => p.UnitId, f => f.PickRandom(dbUnits).Id)
                .RuleFor(p => p.CreatedOn, DateTime.UtcNow)
                .Generate(50);

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
        else
        {
            products = await context.Products.ToListAsync();
        }

        // ================= STOCK =================
        if (!await context.Stock.AnyAsync())
        {
            var stocks = new Faker<Stock>()
                .RuleFor(s => s.ProductId, f => f.PickRandom(products).Id)
                .RuleFor(s => s.Quantity, f => f.Random.Decimal(10, 100))
                .RuleFor(s => s.MinQuantity, 5)
                .RuleFor(s => s.LastUpdated, DateTime.UtcNow)
                .Generate(products.Count);

            context.Stock.AddRange(stocks);
            await context.SaveChangesAsync();
        }

        // ================= CART =================
        if (!await context.Carts.AnyAsync())
        {
            var carts = users.Select(u => new Cart { UserId = u.Id }).ToList();
            context.Carts.AddRange(carts);
            await context.SaveChangesAsync();
        }

        // ================= ORDERS =================
        if (!await context.Orders.AnyAsync())
        {
            var faker = new Faker();
            var orders = new List<Order>();

            foreach (var user in users)
            {
                orders.Add(new Order
                {
                    OrderNumber = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    Status = OrderStatus.Pending,
                    PaymentMethod = PaymentMethod.CASH,
                    OrderDate = DateTime.UtcNow,
                    ShippingAddress = faker.Address.FullAddress(),
                    ShippingCity = faker.Address.City(),
                    ShippingState = faker.Address.State(),
                    ShippingZipCode = faker.Address.ZipCode()
                });
            }

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }
    }

}