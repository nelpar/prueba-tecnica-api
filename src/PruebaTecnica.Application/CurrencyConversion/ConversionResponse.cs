namespace PruebaTecnica.Application.CurrencyConversion;

public record ConversionResponse(string FromCurrency, string ToCurrency, decimal OriginalAmount, decimal ConvertedAmount);
