namespace TasteBox.Extensions;

public static class SwaggerExtensions
{
    public static WebApplication UseSwaggerDocs(this WebApplication app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{ApiDocuments.Dashboard}/swagger.json", "Dashboard API");
            c.SwaggerEndpoint($"/swagger/{ApiDocuments.Mobile}/swagger.json", "Mobile API");

            c.RoutePrefix = string.Empty;
            c.DisplayRequestDuration();
        });

        return app;
    }
}