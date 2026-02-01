namespace TasteBox.Contracts.Stock;

public class RemoveQuantityRequestValidator : AbstractValidator<RemoveQuantityRequest>
{
    public RemoveQuantityRequestValidator()
    {
        RuleFor(x => x.QuantityToRemove)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Quantity to remove must be greater than zero.");
    }
}