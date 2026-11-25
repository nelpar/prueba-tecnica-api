using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Common.Cqrs;
using PruebaTecnica.Application.Common.Interfaces;
using PruebaTecnica.Application.Currencies.Dtos;

namespace PruebaTecnica.Application.Currencies.Queries.GetCurrencies;

public class GetCurrenciesHandler(IAppDbContext db) : IQueryHandler<GetCurrenciesQuery, List<CurrencyResponse>>
{
    public async Task<List<CurrencyResponse>> Handle(GetCurrenciesQuery query, CancellationToken cancellationToken)
    {
        return await db.Currencies
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .Select(c => new CurrencyResponse(c.Id, c.Code, c.Name, c.RateToBase))
            .ToListAsync(cancellationToken);
    }
}
