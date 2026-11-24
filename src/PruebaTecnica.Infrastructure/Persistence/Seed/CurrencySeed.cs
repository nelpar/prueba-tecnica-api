using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Infrastructure.Persistence.Seed;

public static class CurrencySeed
{
    public static readonly Currency[] Data =
    [
        new Currency { Id = 1, Code = "PYG", Name = "Guaraní paraguayo", RateToBase = 1m },
        new Currency { Id = 2, Code = "USD", Name = "Dólar estadounidense", RateToBase = 7300m },
        new Currency { Id = 3, Code = "EUR", Name = "Euro", RateToBase = 7900m }
    ];
}