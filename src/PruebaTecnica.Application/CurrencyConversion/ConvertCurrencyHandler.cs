using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Exceptions;
using PruebaTecnica.Application.Common.Interfaces;

namespace PruebaTecnica.Application.CurrencyConversion;

public class ConvertCurrencyHandler(IAppDbContext db) : IQueryHandler<ConvertCurrencyQuery, ConversionResponse>
{
    private const int DecimalPlaces = 4;

    public async Task<ConversionResponse> Handle(ConvertCurrencyQuery query, CancellationToken cancellationToken)
    {
        var fromCode = query.FromCurrencyCode.Trim().ToUpperInvariant();
        var toCode = query.ToCurrencyCode.Trim().ToUpperInvariant();

        var currencies = await db.Currencies
            .AsNoTracking()
            .Where(c => c.Code == fromCode || c.Code == toCode)
            .ToListAsync(cancellationToken);

        var from = currencies.FirstOrDefault(c => c.Code == fromCode)
            ?? throw new NotFoundException($"No existe la moneda '{fromCode}'.");
        var to = currencies.FirstOrDefault(c => c.Code == toCode)
            ?? throw new NotFoundException($"No existe la moneda '{toCode}'.");

        try
        {
            var baseAmount = query.Amount * from.RateToBase;
            var convertedAmount = Math.Round(baseAmount / to.RateToBase, DecimalPlaces, MidpointRounding.AwayFromZero);

            return new ConversionResponse(from.Code, to.Code, query.Amount, convertedAmount);
        }
        catch (OverflowException)
        {
            throw new BadRequestException("El resultado de la conversión excede el rango numérico permitido.");
        }
    }
}
