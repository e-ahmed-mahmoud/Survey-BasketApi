namespace SurveyBasket.Contracts.Polls;

public record PollResponse(int Id, string Title, string Summary, bool IsPublished, DateOnly StartAt, DateOnly EndAt, 
string CreatedById, DateTime CreatedOn, string? UpdatedById, DateTime? UpdatedOn );

