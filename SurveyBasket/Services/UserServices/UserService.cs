using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Org.BouncyCastle.Ocsp;
using SurveyBasket.Abstractions.Const;
using SurveyBasket.Extensions.Emails;

namespace SurveyBasket.Services.UserServices;

public class UserService(UserManager<ApplicationUser> userManager, IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Result<UserProfile>> GetAccountInfoAsync(string userId)
    {
        var userProfile = await _userManager.Users.Where(u => u.Id == userId).AsNoTracking().ProjectToType<UserProfile>().SingleAsync();
        return Result.Success(userProfile);
    }

    public async Task<Result> UpdateUserAccountAsync(UpdateAccountRequest request, string userId)
    {
        // var user = await _userManager.Users.Where(u => u.Id == userId).SingleAsync();
        // user = request.Adapt(user);
        // await _userManager.UpdateAsync(user);
        // using ExecutedUpdated will update data in single query in one connection without loading it into Memory
        // unlike selecting user into memory then updated in another query
        var result = await _userManager.Users.Where(u => u.Id == userId)
            .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(u => u.FirstName, request.FirstName)
                            .SetProperty(u => u.LastName, request.LastName));
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);
        if (result.Succeeded)
        {
            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await (from u in _dbContext.Users
                      join ur in _dbContext.UserRoles
                      on u.Id equals ur.UserId
                      join r in _dbContext.Roles
                      on ur.RoleId equals r.Id into roles
                      where roles.Any(r => r.Name != DefaultRoles.Member)
                      select new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled, Roles = roles.Select(r => r.Name).ToList() })
                           .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsDisabled })
                           .Select(u => new UserResponse(u.Key.Id, u.Key.FirstName, u.Key.LastName,
                               u.Key.Email, u.Key.IsDisabled, u.SelectMany(r => r.Roles)))
                           .ToListAsync(cancellationToken);
    }

    public async Task<Result<UserResponse>> GetByIdAsync(string id)
    {

        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(AuthErrors.UserNotCreated);

        var roles = await _userManager.GetRolesAsync(user);
        var userData = (user, roles).Adapt<UserResponse>();
        return Result.Success(userData);
    }

    public async Task<Result<UserResponse>> AddAsync(UserCreateRequest request, CancellationToken cancellationToken)
    {
        var isEmailUsed = await _userManager.FindByEmailAsync(request.Email);
        if (isEmailUsed is not null)
            return Result.Failure<UserResponse>(AuthErrors.DuplicatedEmail);

        var definedRoles = _dbContext.Roles.Where(r => !r.IsDeleted).Select(r => r.Name);

        if (request.Roles.Except(definedRoles).Any())
            return Result.Failure<UserResponse>(RoleError.RoleNotDefined);

        var user = request.Adapt<ApplicationUser>();
        user.UserName = user.Email;
        user.NormalizedEmail = user.Email!.ToUpper();
        var userRes = await _userManager.CreateAsync(user, request.Password);

        if (userRes.Succeeded)
        {
            var rolRes = await _userManager.AddToRolesAsync(user, request.Roles.Distinct());

            if (rolRes.Succeeded)
            {
                return Result.Success((user, request.Roles).Adapt<UserResponse>());
            }
            var errorRole = rolRes.Errors.First();
            return Result.Failure<UserResponse>(new Error(errorRole.Code, errorRole.Description, StatusCodes.Status400BadRequest));
        }

        var error = userRes.Errors.First();
        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> UpdateAsync(string id, UserUpdateRequest request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(AuthErrors.UserNotDefined);

        var definedRoles = _dbContext.Roles.Where(r => !r.IsDeleted).Select(r => r.Name);

        if (request.Roles.Except(definedRoles).Any())
            return Result.Failure(RoleError.RoleNotDefined);

        user = request.Adapt(user);
        var userRes = await _userManager.UpdateAsync(user);

        if (userRes.Succeeded)
        {
            await _dbContext.UserRoles.Where(u => u.UserId == user.Id).ExecuteDeleteAsync(cancellationToken);
            await _userManager.AddToRolesAsync(user, request.Roles);
            var result = (user, request.Roles).Adapt<UserResponse>();
            return Result.Success();
        }

        var error = userRes.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
