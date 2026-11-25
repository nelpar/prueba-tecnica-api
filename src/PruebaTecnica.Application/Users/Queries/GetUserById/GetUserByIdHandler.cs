using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Users.Dtos;

namespace PruebaTecnica.Application.Users.Queries.GetUserById;

public class GetUserByIdHandler(IAppDbContext db) : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await db.Users
            .AsNoTracking()
            .Where(u => u.Id == query.Id)
            .Select(u => new UserResponse(u.Id, u.Name, u.Email, u.IsActive, u.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"No existe el usuario con id {query.Id}.");
    }
}
