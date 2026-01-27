namespace SurveyBasket.Contracts.Users;

public record UserAuthResponse (string Id , string FirstName, string LastName , string Email, 
string Token , int TokenExpiresIn, string RefreshToken, DateTime RefreshTokenExpiresIn);

