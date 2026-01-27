namespace SurveyBasket.Contracts.Polls;

public class CreateRequestValidation : AbstractValidator<PollRequest>
{
    public CreateRequestValidation()
    {
        RuleFor(poll => poll.Title).NotEmpty().WithMessage("{PropertyName} is Required")
        .Length(2, 100).WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters");

        RuleFor(poll => poll.Summary).NotEmpty().WithMessage("{PropertyName} is Required")
        .Length(5, 500).WithMessage("{PropertyName} must be between {MinLength} and {MaxLength} characters");

        RuleFor(x => x.StartAt).NotEmpty().GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now));
        RuleFor(x => x.EndAt).NotEmpty();

        RuleFor(x => x).Must(ValidateEndDate).WithName(nameof (PollRequest.EndAt))
            .WithMessage("{PropertyName} must be greater than StartAt");

    }

    private bool ValidateEndDate(PollRequest poll)
    {
        return poll.EndAt > poll.StartAt;
    }
}
