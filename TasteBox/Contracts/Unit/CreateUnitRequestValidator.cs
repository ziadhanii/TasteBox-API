namespace TasteBox.Contracts.Unit;

public class CreateUnitRequestValidator : AbstractValidator<CreateUnitRequest>
{
    public CreateUnitRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Symbol)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.ConversionFactorToBaseUnit)
            .GreaterThan(0)
            .WithMessage("Conversion factor must be greater than zero.");

        RuleFor(x => x)
            .Must(x => !x.IsBaseUnit || x.ConversionFactorToBaseUnit == 1)
            .WithMessage("Base unit must have conversion factor of 1.");
    }
}
