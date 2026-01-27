using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SurveyBasket.Services.UserServices;

public class UserServices( IHttpContextAccessor httpContextAccessor) : IUserServices
{
    //private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return userId;
    }

}
