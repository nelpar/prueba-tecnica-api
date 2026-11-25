using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;

namespace PruebaTecnica.Application.Users.Commands.DeleteUser;

public class DeleteUserHandler(IAppDbContext db) : ICommandHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([command.Id], cancellationToken)
            ?? throw new NotFoundException($"No existe el usuario con id {command.Id}.");

        db.Users.Remove(user);
        await db.SaveChangesAsync(cancellationToken);
    }
}