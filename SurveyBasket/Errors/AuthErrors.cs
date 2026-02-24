namespace SurveyBasket.Errors;

public class AuthErrors
{
    public static Error InvalidCredentialsError => new("InvalidCredentials", "The provided credentials are invalid.", 400);
    public static Error DuplicatedEmail => new("DuplicatedEmail", "The provided Email used before.", 409);
    public static Error UserNotCreated => new("UserNotCreated", "system can't create user.", 500);
    public static Error UserNotDefined => new("UserNotDefined", "User Not Defined.", StatusCodes.Status404NotFound);
    public static Error InvalidEmailConfirmation => new("InvalidEmailConfirmation", "Invalid Email Confirmation .", 500);

    public static Error EmailNotConfirmed => new("EmailNotConfirmed", "Email Not Confirmed", 400);
    public static Error UserAccountLockedOut => new("UserAccountLockedOut", "User Account LockedOut", 400);
    public static Error UserAccountDisabled => new("UserAccountDisabled", "User Account Disabled", StatusCodes.Status423Locked);
    public static Error EmailConfirmedBefore => new("EmailConfirmedBefore", "The provided Email confrim before.", 409);

    public static Error InvalidEmailConfirmationCode => new("InvalidEmailConfirmationCode", "Invalid Email Confirmation Code", StatusCodes.Status400BadRequest);

}
