using FluentValidation;

namespace SurveyBasket.Validations;

public class CreateRequestValidation : AbstractValidator<CreatePollRequest>
{
    public CreateRequestValidation()
    {
        RuleFor(poll => poll.Title).NotEmpty().WithMessage("{PropertyName} is Required")
        .Length(2, 100).WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters");

        RuleFor(poll => poll.Descripation).NotEmpty().WithMessage("{PropertyName} is Required")
        .Length(5, 500).WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters");



    }
}
