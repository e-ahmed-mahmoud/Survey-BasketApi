namespace SurveyBasket.Errors;

public class AuthErrors
{
    public static Error InvalidCredentialsError => new("InvalidCredentials", "The provided credentials are invalid.");

}
