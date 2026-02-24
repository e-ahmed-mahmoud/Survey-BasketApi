namespace SurveyBasket.Contracts.Users;

public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
{
    public UserUpdateRequestValidator()
    {
        RuleFor(u => u.FirstName).NotEmpty().Length(2, 155);
        RuleFor(u => u.LastName).NotEmpty().Length(2, 155);

        RuleFor(u => u.Roles).NotNull().NotEmpty();
        RuleFor(u => u.Roles).Must(x => x.Distinct().Count() > 0).When(x => x.Roles is not null);
    }

}