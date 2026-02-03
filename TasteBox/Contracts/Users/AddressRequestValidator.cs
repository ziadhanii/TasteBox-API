namespace TasteBox.Contracts.Users;

public class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.BuildingNumber)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.BuildingNumber));

        RuleFor(x => x.Floor)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.Floor));

        RuleFor(x => x.Apartment)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.Apartment));

        RuleFor(x => x.Landmark)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Landmark));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
