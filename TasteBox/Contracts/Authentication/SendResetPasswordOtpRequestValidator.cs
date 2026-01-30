namespace TasteBox.Contracts.Authentication;

public class SendResetPasswordOtpRequestValidator : AbstractValidator<SendResetPasswordOtpRequest>
{
    public SendResetPasswordOtpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
