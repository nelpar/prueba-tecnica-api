using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Address> Addresses { get; }
    DbSet<Currency> Currencies { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}