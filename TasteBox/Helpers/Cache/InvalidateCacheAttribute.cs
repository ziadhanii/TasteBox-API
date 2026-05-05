namespace TasteBox.Helpers.Cache;

[AttributeUsage(AttributeTargets.Method)]
public class InvalidateCacheAttribute(params string[] keyNamespaces) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (ShouldInvalidate(resultContext))
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var namespacesToInvalidate = keyNamespaces.Length == 0 ? [] : keyNamespaces;

            foreach (var keyNamespace in namespacesToInvalidate)
            {
                var pattern = CacheRequestResolver.CreateUserScopedPattern(context.HttpContext, keyNamespace);
                await cacheService.RemoveCacheByPatternAsync(pattern);
            }
        }
    }

    private static bool ShouldInvalidate(ActionExecutedContext context)
    {
        if (context.Exception is not null && !context.ExceptionHandled)
            return false;

        return context.Result switch
        {
            OkObjectResult => true,
            OkResult => true,
            NoContentResult => true,
            ObjectResult { StatusCode: >= 200 and < 300 } => true,
            StatusCodeResult { StatusCode: >= 200 and < 300 } => true,
            _ => false
        };
    }
}
