using System.Security.Cryptography;
using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SurveyBasket.Abstractions.Const;
using SurveyBasket.Authentication;
using SurveyBasket.Extensions.Emails;

namespace SurveyBasket.Services.UserServices;

public class AuthServices(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider,
SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor,
ILogger<AuthServices> logger, RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext) : IAuthServices
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<AuthServices> _logger = logger;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Result<UserAuthResponse>> GetAuthTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        // check if user exisits 
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure<UserAuthResponse>(AuthErrors.InvalidCredentialsError);
        if (user.IsDisabled)
            return Result.Failure<UserAuthResponse>(AuthErrors.UserAccountDisabled);

        var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, true);
        if (signInResult.IsNotAllowed || signInResult.IsLockedOut)
            return Result.Failure<UserAuthResponse>(signInResult.IsNotAllowed ? AuthErrors.EmailNotConfirmed : AuthErrors.UserAccountLockedOut);
        //generate Token
        var refreshTokenExpireInDays = 14; // 14 days for expiry

        string refreshToken = GetRefreshToken();
        //save refresh token and expiry to db
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = DateTime.UtcNow.AddDays(refreshTokenExpireInDays)
        });
        await _userManager.UpdateAsync(user);
        var (roles, permissionsClaims) = await GetUserRolesAndPermissionsAsync(user);
        //generate jwt token\
        (string token, int expiresIn) tokenData = _jwtProvider.GenerateJWTToken(user, roles, permissionsClaims);
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
        if (user == null)
            return Result.Failure<UserAuthResponse>(AuthErrors.InvalidCredentialsError);

        if (user.IsDisabled || user.LockoutEnabled)
            return Result.Failure<UserAuthResponse>(user.IsDisabled ? AuthErrors.UserAccountDisabled : AuthErrors.UserAccountLockedOut);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken && t.IsActive);
        if (userRefreshToken == null) return Result.Failure<UserAuthResponse>(AuthErrors.InvalidCredentialsError);

        //return new toekn and new refresh token
        var (roles, permissionsClaims) = await GetUserRolesAndPermissionsAsync(user);
        var (newToken, expiresIn) = _jwtProvider.GenerateJWTToken(user, roles, permissionsClaims);
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


    public async Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken = default)
    {
        var isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == registerRequest.Email, cancellationToken);
        if (isEmailExists)
            return Result.Failure<UserAuthResponse>(AuthErrors.DuplicatedEmail);

        var user = registerRequest.Adapt<ApplicationUser>();

        var res = await _userManager.CreateAsync(user, registerRequest.Password);

        if (res.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("Conformition token for Emial:  {code}", code);
            await SendConfrimationEmailAsync(user, code);

            return Result.Success();
        }
        return Result.Failure(new Error(res.Errors.First().Code, res.Errors.First().Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> ConfirmEmail(EmailConfirm request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(AuthErrors.InvalidEmailConfirmation with { StatusCode = 404 });

        if (user.EmailConfirmed)
            return Result.Failure(AuthErrors.EmailConfirmedBefore);
        var code = request.Code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(AuthErrors.InvalidEmailConfirmationCode);
        }
        var res = await _userManager.ConfirmEmailAsync(user, code); //verify token & update user in Db
        if (res.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
            return Result.Success();
        }
        return Result.Failure(new Error(res.Errors.First().Code, res.Errors.First().Description, StatusCodes.Status406NotAcceptable));
    }

    public async Task<Result> ResendEmailConfirmationCode(ResendConfirmationEmailCodeRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();    //not notify user that email not define

        if (user.EmailConfirmed)
            return Result.Failure(AuthErrors.EmailConfirmedBefore);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        _logger.LogInformation("Regenrated EMail confirma code : {code} ", code);
        await SendConfrimationEmailAsync(user, code);
        return Result.Success();

    }
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {

        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure(AuthErrors.InvalidCredentialsError);

        var result = new IdentityResult();
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.Password);
        }
        catch (FormatException)
        {
            return Result.Failure(AuthErrors.InvalidCredentialsError);
        }

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ForgetPasswordAsync(string userId, ForgetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure(AuthErrors.InvalidEmailConfirmation);

        var code = await _userManager.GeneratePasswordResetTokenAsync(user!);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var orgin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var placeHolder = new Dictionary<string, string>
        {
            {"{{userName}}", user.FirstName},
            {"{{VerifyEndpointUrl}}", $"{orgin}/api/UserAccount/ResetUserPassword?email={user.Email}&code=${code}"}
        };
        var emailMessage = EmailBodyBuilder.GenerateEmailBody("ResetPassword-template", placeHolder);
        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Reset Password", emailMessage));

        return Result.Success();
    }


    private string GetRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private async Task SendConfrimationEmailAsync(ApplicationUser user, string code)
    {
        var orgin = _httpContextAccessor.HttpContext!.Request.Headers.Origin;
        var messageBody = EmailBodyBuilder.GenerateEmailBody(EmailBodyBuilder.EmailConfirmationHtmlTemplateName,
            new Dictionary<string, string>
            {
                {"{{userName}}",user.FirstName},
                { "{{VerifyEndpointUrl}}", $"{orgin}/api/Auth/ConfirmEmail?userId={user.Id}&code={code}" }
            }
        );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, $"Basket Suvery Register Email Confirmation", messageBody));
        await Task.CompletedTask;
    }

    private async Task<(IEnumerable<string>, IEnumerable<string>)> GetUserRolesAndPermissionsAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        // var permissionsClaims = roles.Select(async r => await _roleManager.FindByNameAsync(r))
        //               .Select(async r => await  _roleManager.GetClaimsAsync(r.GetAwaiter().GetResult()));
        //getting it using Extensions methods
        // var permissionsClaims = _dbContext.RoleClaims.Join(_dbContext.Roles, p => p.RoleId,
        // r => r.Id, (p, r) => new { r.Name, p.ClaimValue }).Where(r => roles.Contains(r.Name!)).Select(x => x.ClaimValue).Distinct();
        //best and more reabable is using Query Syantax, use Distinct to avoud Duplication
        var permissionsClaims = await (from r in _dbContext.Roles
                                       join p in _dbContext.RoleClaims on r.Id equals p.RoleId
                                       where roles.Contains(r.Name!)
                                       select p.ClaimValue).Distinct().ToListAsync();
        return (roles, permissionsClaims);
    }

}
