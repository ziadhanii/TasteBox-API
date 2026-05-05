namespace TasteBox.Contracts.Authentication;

public class GoogleLoginRequestValidator : AbstractValidator<GoogleLoginRequest>
{
    public GoogleLoginRequestValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty()
            .MinimumLength(10);
    }
}