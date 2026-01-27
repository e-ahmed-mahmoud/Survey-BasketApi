namespace SurveyBasket.Contracts.Users;

public record AuthRequest(
    string Email,
    string Password
);
