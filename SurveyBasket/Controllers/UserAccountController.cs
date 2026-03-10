using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Services.UserServices;

namespace SurveyBasket.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserAccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAccountInfo()
    {
        var result = await _userService.GetAccountInfoAsync(User.GetUserId()!);
        return Ok(result.Value);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> UpdateUserAccount(UpdateAccountRequest request)
    {
        var result = await _userService.UpdateUserAccountAsync(request, User.GetUserId()!);
        return NoContent();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ChangeUserPassword(ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);
        return result.IsSuccess ? NoContent() : result.ToProblem(result.Error.StatusCode);
    }


}
