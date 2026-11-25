using FluentValidation;

namespace PruebaTecnica.Application.Currencies.Commands.CreateCurrency;

public class CreateCurrencyValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .Matches("^[A-Za-z]{3}$").WithMessage("El código debe tener 3 letras (ISO 4217, ej: USD).");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100);

        RuleFor(x => x.RateToBase)
            .GreaterThan(0).WithMessage("La tasa debe ser mayor a 0.");
    }
}
