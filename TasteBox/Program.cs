using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using TasteBox.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

FirebaseApp.Create(new AppOptions
{
    Credential = CredentialFactory
        .FromFile<ServiceAccountCredential>("tastebox-c4cbf-firebase-adminsdk-fbsvc-8326fbf18b.json")
        .ToGoogleCredential()
});

var credential = CredentialFactory
    .FromFile<ServiceAccountCredential>("tastebox-c4cbf-firebase-adminsdk-fbsvc-8326fbf18b.json")
    .ToGoogleCredential()
    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

var accessToken = await credential
    .UnderlyingCredential
    .GetAccessTokenForRequestAsync();

Console.WriteLine(accessToken);


var app = builder.Build();

app.UseSwaggerDocs();

app.UseScalarDocs();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context, userManager);
}

app.Run();