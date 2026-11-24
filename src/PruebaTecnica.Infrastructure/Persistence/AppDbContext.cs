using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
