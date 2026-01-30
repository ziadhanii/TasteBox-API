namespace TasteBox.Abstractions.Consts;

public static class Permissions
{
    public static string Type { get; } = "permissions";

    public const string GetProducts = "products:read";
    public const string AddProducts = "products:add";
    public const string UpdateProducts = "products:update";
    public const string DeleteProducts = "products:delete";

    public const string GetCategories = "categories:read";
    public const string AddCategories = "categories:add";
    public const string UpdateCategories = "categories:update";
    public const string DeleteCategories = "categories:delete";

    public const string GetStock = "stock:read";
    public const string AddStock = "stock:add";
    public const string UpdateStock = "stock:update";
    public const string DeleteStock = "stock:delete";

    public const string GetUsers = "users:read";
    public const string AddUsers = "users:add";
    public const string UpdateUsers = "users:update";

    public const string GetRoles = "roles:read";
    public const string AddRoles = "roles:add";
    public const string UpdateRoles = "roles:update";

    public const string Results = "results:read";

    public static IList<string?> GetAllPermissions() =>
        typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
}