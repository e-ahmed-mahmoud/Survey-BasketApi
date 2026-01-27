namespace SurveyBasket.Entities;

public sealed class Poll : AuditLogging
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateOnly StartAt { get; set; }
    public DateOnly EndAt { get; set; }

    public bool IsPublished { get; set; }

}
