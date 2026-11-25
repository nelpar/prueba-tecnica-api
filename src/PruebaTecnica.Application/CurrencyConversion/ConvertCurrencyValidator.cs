using FluentValidation;

namespace PruebaTecnica.Application.CurrencyConversion;

public class ConvertCurrencyValidator : AbstractValidator<ConvertCurrencyQuery>
{
    public ConvertCurrencyValidator()
    {
        RuleFor(x => x.FromCurrencyCode)
            .NotEmpty().WithMessage("La moneda de origen es obligatoria.");

        RuleFor(x => x.ToCurrencyCode)
            .NotEmpty().WithMessage("La moneda de destino es obligatoria.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.")
            .LessThanOrEqualTo(1_000_000_000_000m).WithMessage("El monto no puede superar 1.000.000.000.000.");
    }
}
