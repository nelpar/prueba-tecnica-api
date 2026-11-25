namespace PruebaTecnica.Application.CurrencyConversion;

public record ConvertCurrencyQuery(string FromCurrencyCode, string ToCurrencyCode, decimal Amount);
