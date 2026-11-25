namespace PruebaTecnica.Application.Currencies.Commands.CreateCurrency;

public record CreateCurrencyCommand(string Code, string Name, decimal RateToBase);
