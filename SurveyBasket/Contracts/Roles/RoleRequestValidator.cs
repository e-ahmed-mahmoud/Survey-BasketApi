namespace SurveyBasket.Contracts.Roles;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().Length(2, 255);

        RuleFor(r => r.Permissions).NotNull().NotEmpty();

        RuleFor(r => r.Permissions).Must(p => p.Distinct().Count() == p.Count).When(x => x.Permissions is not null)
        .WithMessage("duplicated permission not allowed");
    }
}