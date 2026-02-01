namespace TasteBox.Contracts.Stock;

public class AddQuantityRequestValidator : AbstractValidator<AddQuantityRequest>
{
    public AddQuantityRequestValidator()
    {
        RuleFor(x => x.QuantityToAdd)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Quantity to add must be greater than zero.");
    }
}