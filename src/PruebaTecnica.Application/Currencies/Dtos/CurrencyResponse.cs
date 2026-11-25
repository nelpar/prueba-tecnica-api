using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Application.Currencies.Dtos;

public record CurrencyResponse(int Id, string Code, string Name, decimal RateToBase)
{
    public static CurrencyResponse FromEntity(Currency currency) =>
        new(currency.Id, currency.Code, currency.Name, currency.RateToBase);
}
