using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Infrastructure;
using PruebaTecnica.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/", () => "PruebaTecnica API");

app.Run();