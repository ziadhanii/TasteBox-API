using Microsoft.AspNetCore.Mvc.Filters;

namespace TasteBox.Helpers.Cache;

[AttributeUsage(AttributeTargets.Method)]
public class InvalidateCacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _pattern;

    public InvalidateCacheAttribute(string pattern)
    {
        _pattern = pattern;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (resultContext.Exception == null || resultContext.ExceptionHandled)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            // Add user ID to pattern to invalidate user-specific cache
            var userId = context.HttpContext.User.GetUserId();
            var userPattern = !string.IsNullOrEmpty(userId) ? $"user:{userId}" : "";

            var datePattern = $"{_pattern}_{userPattern}_{DateTime.UtcNow.Date:yyyyMMdd}";
            await cacheService.RemoveCacheByPatternAsync(datePattern);
        }
    }
}
