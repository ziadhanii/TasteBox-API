namespace TasteBox.Helpers;

public static class CacheKeys
{
    private const string Prefix = "tb";
    private const string Version = "v1";

    public const string Cart = Prefix + ":" + Version + ":cart";
    public const string CartItemsCount = Prefix + ":" + Version + ":cart:items-count";
    public const string Products = Prefix + ":" + Version + ":products";
    public const string Orders = Prefix + ":" + Version + ":orders";

    public static string EmailVerificationOtp(string email)
        => $"{Prefix}:{Version}:auth:email_verification:otp:{Normalize(email)}";

    public static string EmailVerificationRateLimit(string email)
        => $"{Prefix}:{Version}:auth:email_verification:rate_limit:{Normalize(email)}";

    public static string PasswordResetOtp(string email)
        => $"{Prefix}:{Version}:auth:password_reset:otp:{Normalize(email)}";

    public static string PasswordResetToken(string email)
        => $"{Prefix}:{Version}:auth:password_reset:token:{Normalize(email)}";

    public static string PasswordResetRateLimit(string email)
        => $"{Prefix}:{Version}:auth:password_reset:rate_limit:{Normalize(email)}";

    public static string CartKey(string userId)
        => BuildUserScopedKey(Cart, userId);

    public static string CartPattern(string userId)
        => BuildUserScopedPattern(Cart, userId);

    public static string CartItemsCountKey(string userId)
        => BuildUserScopedKey(CartItemsCount, userId);

    public static string CartItemsCountPattern(string userId)
        => BuildUserScopedPattern(CartItemsCount, userId);

    public static string ProductsPattern()
        => $"{Products}*";

    public static string OrdersKey(string userId)
        => BuildUserScopedKey(Orders, userId);

    public static string OrdersPattern(string userId)
        => BuildUserScopedPattern(Orders, userId);

    public static string BuildUserScopedKey(string keyNamespace, string userId)
        => $"{keyNamespace}:user:{userId}";

    public static string BuildUserScopedKey(string keyNamespace, string userId, IQueryCollection query)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyNamespace);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentNullException.ThrowIfNull(query);

        var builder = new StringBuilder(BuildUserScopedKey(keyNamespace, userId));

        foreach (var (key, value) in query.OrderBy(entry => entry.Key))
        {
            builder.Append($"|{key}-{value}");
        }

        return builder.ToString();
    }

    public static string BuildUserScopedPattern(string keyNamespace, string userId)
        => $"{BuildUserScopedKey(keyNamespace, userId)}*";

    private static string Normalize(string value)
        => value.Trim().ToLowerInvariant();
}
