namespace SurveyBasket.Abstractions;

public record Error(string ErrorCode, string ErrorMessage, int StatusCode = 400)
{
    //default property for no error to return in case of success
    public static readonly Error None = new(string.Empty, string.Empty);
}
