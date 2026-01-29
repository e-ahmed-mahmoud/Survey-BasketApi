namespace SurveyBasket.Errors;

public class PollErrors
{
    public static Error NotDefinedError => new("PollNotFound", "The specified poll was not found.", 404);

    public static Error NotPublishedError => new("PollNotPublished", "The specified poll is not published.", 400);

    public static Error InvalidPollDataError => new("InvalidPollData", "The provided poll data is invalid.", 400);
    public static Error DuplicateTitleError => new("DuplicatePollTitle", "A poll with the same title already exists.", 409);
}
