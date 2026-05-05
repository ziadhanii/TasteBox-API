namespace TasteBox.Helpers.Cache;

[AttributeUsage(AttributeTargets.Method)]
public class CacheAttribute(int timeToLiveMinutes, string keyNamespace) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        var cacheKey = CacheRequestResolver.CreateUserScopedKey(context.HttpContext.Request, keyNamespace);

        var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            context.Result = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
            return;
        }

        var executedContext = await next();

        if (executedContext.Result is OkObjectResult okObjectResult && okObjectResult.Value != null)
        {
            await cacheService.CacheResponseAsync(
                cacheKey,
                okObjectResult.Value,
                TimeSpan.FromMinutes(timeToLiveMinutes));
        }
    }
}
