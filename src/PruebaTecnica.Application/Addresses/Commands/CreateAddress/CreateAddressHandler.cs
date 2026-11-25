using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Addresses.Dtos;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Addresses.Commands.CreateAddress;

public class CreateAddressHandler(IAppDbContext db) : ICommandHandler<CreateAddressCommand, AddressResponse>
{
    public async Task<AddressResponse> Handle(CreateAddressCommand command, CancellationToken cancellationToken)
    {
        if (!await db.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken))
            throw new NotFoundException($"No existe el usuario con id {command.UserId}.");

        var address = new Address
        {
            UserId = command.UserId,
            Street = command.Street.Trim(),
            City = command.City.Trim(),
            Country = command.Country.Trim(),
            ZipCode = string.IsNullOrWhiteSpace(command.ZipCode) ? null : command.ZipCode.Trim()
        };

        db.Addresses.Add(address);
        await db.SaveChangesAsync(cancellationToken);

        return AddressResponse.FromEntity(address);
    }
}
