using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Users.Dtos;

namespace PruebaTecnica.Application.Users.Queries.GetUsers;

public class GetUsersHandler(IAppDbContext db) : IQueryHandler<GetUsersQuery, List<UserResponse>>
{
    public async Task<List<UserResponse>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = db.Users.AsNoTracking();

        if (query.IsActive is not null)
            users = users.Where(u => u.IsActive == query.IsActive);

        return await users
            .OrderBy(u => u.Id)
            .Select(u => new UserResponse(u.Id, u.Name, u.Email, u.IsActive, u.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
