using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Api.Middleware;
using PruebaTecnica.Api.Security;
using PruebaTecnica.Application;
using PruebaTecnica.Infrastructure;
using PruebaTecnica.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();
app.UseMiddleware<ApiKeyMiddleware>();

app.MapGet("/", () => "PruebaTecnica API");

app.Run();
