namespace TasteBox.Contracts.Authentication;

public class VerifyResetPasswordOtpRequestValidator : AbstractValidator<VerifyResetPasswordOtpRequest>
{
    public VerifyResetPasswordOtpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Otp)
            .NotEmpty()
            .Length(6)
            .Matches(@"^\d{6}$").WithMessage("OTP must be 6 digits");
    }
}
