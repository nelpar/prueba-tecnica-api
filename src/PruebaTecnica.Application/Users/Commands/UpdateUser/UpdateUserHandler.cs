using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Users.Dtos;

namespace PruebaTecnica.Application.Users.Commands.UpdateUser;

public class UpdateUserHandler(IAppDbContext db) : ICommandHandler<UpdateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([command.Id], cancellationToken)
            ?? throw new NotFoundException($"No existe el usuario con id {command.Id}.");

        var email = command.Email.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(u => u.Email == email && u.Id != command.Id, cancellationToken))
            throw new ConflictException($"Ya existe otro usuario con el email '{email}'.");

        user.Name = command.Name.Trim();
        user.Email = email;
        user.IsActive = command.IsActive ?? user.IsActive;

        await db.SaveChangesAsync(cancellationToken);

        return UserResponse.FromEntity(user);
    }
}