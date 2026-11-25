using PruebaTecnica.Application.Addresses.Dtos;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;

namespace PruebaTecnica.Application.Addresses.Commands.UpdateAddress;

public class UpdateAddressHandler(IAppDbContext db) : ICommandHandler<UpdateAddressCommand, AddressResponse>
{
    public async Task<AddressResponse> Handle(UpdateAddressCommand command, CancellationToken cancellationToken)
    {
        var address = await db.Addresses.FindAsync([command.Id], cancellationToken)
            ?? throw new NotFoundException($"No existe la dirección con id {command.Id}.");

        address.Street = command.Street.Trim();
        address.City = command.City.Trim();
        address.Country = command.Country.Trim();
        address.ZipCode = string.IsNullOrWhiteSpace(command.ZipCode) ? null : command.ZipCode.Trim();

        await db.SaveChangesAsync(cancellationToken);

        return AddressResponse.FromEntity(address);
    }
}
