using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Addresses.Dtos;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;

namespace PruebaTecnica.Application.Addresses.Queries.GetAddressesByUser;

public class GetAddressesByUserHandler(IAppDbContext db)
    : IQueryHandler<GetAddressesByUserQuery, List<AddressResponse>>
{
    public async Task<List<AddressResponse>> Handle(GetAddressesByUserQuery query, CancellationToken cancellationToken)
    {
        if (!await db.Users.AnyAsync(u => u.Id == query.UserId, cancellationToken))
            throw new NotFoundException($"No existe el usuario con id {query.UserId}.");

        return await db.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == query.UserId)
            .OrderBy(a => a.Id)
            .Select(a => new AddressResponse(a.Id, a.UserId, a.Street, a.City, a.Country, a.ZipCode))
            .ToListAsync(cancellationToken);
    }
}
