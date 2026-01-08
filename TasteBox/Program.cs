var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();


app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty;
    c.DisplayRequestDuration();
});
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler();


app.Run();