namespace TasteBox.Extensions;

public static class ScalarExtensions
{
    public static WebApplication UseScalarDocs(this WebApplication app)
    {
        app.MapScalarApiReference("/docs", options =>
        {
            options
                .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json")
                .WithTitle("TasteBox API Reference")
                .WithTheme(ScalarTheme.BluePlanet)
                .SortTagsAlphabetically()
                .AlwaysShowDeveloperTools()
                .HideModels = false;

            options.AddDocument(ApiDocuments.Dashboard, "Dashboard",
                $"/swagger/{ApiDocuments.Dashboard}/swagger.json");

            options.AddDocument(ApiDocuments.Mobile, "Mobile",
                $"/swagger/{ApiDocuments.Mobile}/swagger.json");

            options.AddPreferredSecuritySchemes("bearer")
                .AddHttpAuthentication("bearer", auth =>
                {
                    auth.Token = "{your JWT token}";
                    auth.Description = "JWT Authorization header using the Bearer scheme.";
                });
        });

        return app;
    }
}