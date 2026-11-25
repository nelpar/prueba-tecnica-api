using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Users.Dtos;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Users.Commands.CreateUser;

public class CreateUserHandler(IAppDbContext db, IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(u => u.Email == email, cancellationToken))
            throw new ConflictException($"Ya existe un usuario con el email '{email}'.");

        var user = new User
        {
            Name = command.Name.Trim(),
            Email = email,
            PasswordHash = passwordHasher.Hash(command.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return UserResponse.FromEntity(user);
    }
}