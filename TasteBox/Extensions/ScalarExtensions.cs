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

            options.AddDocument(APIDocuments.Dashboard, "Dashboard",
                $"/swagger/{APIDocuments.Dashboard}/swagger.json");

            options.AddDocument(APIDocuments.Mobile, "Mobile",
                $"/swagger/{APIDocuments.Mobile}/swagger.json");

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