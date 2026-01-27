namespace SurveyBasket.Services.UserServices;

public interface IAuthServices
{
    Task<UserAuthResponse?> GetAuthTokenAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<UserAuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

    
}
