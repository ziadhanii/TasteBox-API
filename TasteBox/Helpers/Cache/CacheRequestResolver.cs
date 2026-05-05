namespace TasteBox.Helpers.Cache;

public static class CacheRequestResolver
{
    public static string GetUserId(HttpContext httpContext)
        => httpContext.User.GetUserId() ?? "anonymous";

    public static string CreateUserScopedKey(HttpRequest request, string keyNamespace)
        => CacheKeys.BuildUserScopedKey(keyNamespace, GetUserId(request.HttpContext), request.Query);

    public static string CreateUserScopedPattern(HttpContext httpContext, string keyNamespace)
        => CacheKeys.BuildUserScopedPattern(keyNamespace, GetUserId(httpContext));
}
