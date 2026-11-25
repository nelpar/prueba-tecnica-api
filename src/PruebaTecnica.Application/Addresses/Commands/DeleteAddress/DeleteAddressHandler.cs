using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;

namespace PruebaTecnica.Application.Addresses.Commands.DeleteAddress;

public class DeleteAddressHandler(IAppDbContext db) : ICommandHandler<DeleteAddressCommand>
{
    public async Task Handle(DeleteAddressCommand command, CancellationToken cancellationToken)
    {
        var address = await db.Addresses.FindAsync([command.Id], cancellationToken)
            ?? throw new NotFoundException($"No existe la dirección con id {command.Id}.");

        db.Addresses.Remove(address);
        await db.SaveChangesAsync(cancellationToken);
    }
}
