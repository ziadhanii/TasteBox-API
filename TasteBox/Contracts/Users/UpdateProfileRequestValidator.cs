namespace TasteBox.Contracts.Users;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .Length(3, 100)
            .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

        RuleFor(x => x.LastName)
            .Length(3, 100)
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.UserName)
            .Length(3, 50)
            .When(x => !string.IsNullOrWhiteSpace(x.UserName));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(010|011|012|015)[0-9]{8}$")
            .WithMessage("Phone number must be a valid Egyptian phone number")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}