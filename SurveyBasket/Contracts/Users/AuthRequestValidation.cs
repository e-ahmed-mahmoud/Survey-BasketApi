namespace SurveyBasket.Contracts.Users;

public class AuthRequestValidation : AbstractValidator<AuthRequest>
{
    public AuthRequestValidation()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(18).Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,18}$")
            .WithMessage("Password must be 8-18 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

    }

}
