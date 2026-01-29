using System.Security.Claims;

namespace SurveyBasket.Services.UserServices;

public class UserServices(IHttpContextAccessor httpContextAccessor) : IUserServices
{
    //private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return userId;
    }

}
