using FluentValidation;

namespace PruebaTecnica.Application.Addresses.Commands.CreateAddress;

public class CreateAddressValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("La calle es obligatoria.")
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("La ciudad es obligatoria.")
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("El país es obligatorio.")
            .MaximumLength(100);

        RuleFor(x => x.ZipCode)
            .MaximumLength(20);
    }
}
