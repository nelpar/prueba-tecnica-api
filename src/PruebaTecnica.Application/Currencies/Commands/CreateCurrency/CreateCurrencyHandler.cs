using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Currencies.Dtos;
using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Currencies.Commands.CreateCurrency;

public class CreateCurrencyHandler(IAppDbContext db) : ICommandHandler<CreateCurrencyCommand, CurrencyResponse>
{
    public async Task<CurrencyResponse> Handle(CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var code = command.Code.Trim().ToUpperInvariant();

        if (await db.Currencies.AnyAsync(c => c.Code == code, cancellationToken))
            throw new ConflictException($"Ya existe una moneda con el código '{code}'.");

        var currency = new Currency
        {
            Code = code,
            Name = command.Name.Trim(),
            RateToBase = command.RateToBase
        };

        db.Currencies.Add(currency);
        await db.SaveChangesAsync(cancellationToken);

        return CurrencyResponse.FromEntity(currency);
    }
}
