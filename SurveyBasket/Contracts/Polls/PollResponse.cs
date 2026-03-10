namespace SurveyBasket.Contracts.Polls;

public record PollResponse(int Id, string Title, string Summary, bool IsPublished, DateOnly StartAt, DateOnly EndAt,
string CreatedById, DateTime CreatedOn, string? UpdatedById, DateTime? UpdatedOn);

public record PollResponseV2(int Id, string Title, string Summary, DateOnly StartAt, DateOnly EndAt,
string CreatedById, DateTime CreatedOn, string? UpdatedById, DateTime? UpdatedOn);
