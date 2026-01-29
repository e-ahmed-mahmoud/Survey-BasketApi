
using System.Security.Cryptography;
using SurveyBasket.Authentication;

namespace SurveyBasket.Services.UserServices;

public class AuthServices(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthServices
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    public async Task<Result<UserAuthResponse>> GetAuthTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        // check if user exisits 
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure<UserAuthResponse>(AuthErrors.InvalidCredentialsError);
        //check password
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
            return Result.Failure<UserAuthResponse>(AuthErrors.InvalidCredentialsError);

        var refreshTokenExpireInDays = 14; // 14 days for expiry

        string refreshToken = GetRefreshToken();
        //save refresh token and expiry to db
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = DateTime.UtcNow.AddDays(refreshTokenExpireInDays)
        });
        await _userManager.UpdateAsync(user);

        //generate jwt token\
        (string token, int expiresIn) tokenData = _jwtProvider.GenerateJWTToken(user);
        //return user auth response
        var response = new UserAuthResponse(user?.Id!, user?.FirstName!, user?.LastName!,
            user?.Email!, tokenData.token, tokenData.expiresIn, RefreshToken: refreshToken,
            RefreshTokenExpiresIn: DateTime.UtcNow.AddDays(refreshTokenExpireInDays));
        return Result.Success(response);
    }

    public async Task<Result<UserAuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userID = _jwtProvider.ValidateToken(token);
        if (userID == null) return Result.Failure<UserAuthResponse>(AuthErrors.InvalidCredentialsError);

        var user = await _userManager.FindByIdAsync(userID);
        if (user == null) return Result.Failure<UserAuthResponse>(AuthErrors.InvalidCredentialsError);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);
        if (userRefreshToken == null) return Result.Failure<UserAuthResponse>(AuthErrors.InvalidCredentialsError);

        //return new toekn and new refresh token
        var (newToken, expiresIn) = _jwtProvider.GenerateJWTToken(user);
        var newRefreshToken = GetRefreshToken();
        var refreshTokenExpireInDays = 14; // 14 days for expiry

        //save new refresh token intto db, and revoke old one
        user.RefreshTokens.Add(new RefreshToken { Token = newRefreshToken, ExpiresOn = DateTime.UtcNow.AddDays(refreshTokenExpireInDays) });
        userRefreshToken.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        var response = new UserAuthResponse(user?.Id!, user?.FirstName!, user?.LastName!,
            user?.Email!, newToken, expiresIn, newRefreshToken,
            DateTime.UtcNow.AddDays(refreshTokenExpireInDays));
        return Result.Success(response);
    }

    public async Task<bool> RevokeRefreshTokenByUserId(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return true;
        var userRefreshToken = user?.RefreshTokens.SingleOrDefault(t => t.IsActive);
        if (userRefreshToken is null) return true;
        userRefreshToken?.RevokedOn = DateTime.UtcNow;
        await _userManager.UpdateAsync(user!);
        return true;

    }

    private static string GetRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
