using SurveyBasket.Abstractions.Const;

namespace SurveyBasket.Contracts.Users;

public class UserCreateRequestValidator : AbstractValidator<UserCreateRequest>
{
    public UserCreateRequestValidator()
    {
        RuleFor(u => u.FirstName).NotEmpty().Length(2, 155);
        RuleFor(u => u.LastName).NotEmpty().Length(2, 155);
        RuleFor(u => u.Email).NotEmpty().EmailAddress().Length(2, 155);
        RuleFor(u => u.Password).NotEmpty().Matches(RegexPatterns.PasswordPattern);

        RuleFor(u => u.Roles).NotNull().NotEmpty();
        RuleFor(u => u.Roles).Must(x => x.Distinct().Count() > 0).When(x => x.Roles is not null);
    }

}
