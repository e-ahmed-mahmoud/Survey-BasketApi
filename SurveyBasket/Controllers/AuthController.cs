using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Services.UserServices;

namespace SurveyBasket.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthServices authService) : ControllerBase
{
    private readonly IAuthServices _authService = authService;

    [HttpPost("[action]")]
    public async Task<IActionResult> LoginAsync([FromBody] AuthRequest authRequest, CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.GetAuthTokenAsync(authRequest.Email, authRequest.Password, cancellationToken);

        return authResult == null ? BadRequest("Invalid credentials") : Ok(authResult);
    }

    [HttpPost("RefreshAuth")]
    public async Task<IActionResult> RefreshAuthAsync([FromBody] RefreshTokenRequest request)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token , request.RefreshToken);

        return authResult == null ? BadRequest("invalid tokens") : Ok (authResult);
    }

}
