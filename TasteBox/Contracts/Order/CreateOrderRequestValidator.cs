namespace TasteBox.Contracts.Order;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("Invalid payment method");

        RuleFor(x => x.AddressId)
            .GreaterThan(0)
            .When(x => x.AddressId.HasValue)
            .WithMessage("Address ID must be greater than zero");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");
    }
}
