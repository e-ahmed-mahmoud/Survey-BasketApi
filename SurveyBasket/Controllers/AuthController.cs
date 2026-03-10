using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Services.UserServices;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthServices authService) : ControllerBase
{
    private readonly IAuthServices _authService = authService;

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] AuthRequest authRequest, CancellationToken cancellationToken = default)
    {
        var authResult = await _authService.GetAuthTokenAsync(authRequest.Email, authRequest.Password, cancellationToken);

        return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest(authResult.Error);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RefreshAuth([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult.IsSuccess ? Ok(authResult.Value) : BadRequest(authResult.Error);
    }
    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest, CancellationToken cancellationToken)
    {
        var authResult = await _authService.RegisterAsync(registerRequest, cancellationToken);

        return authResult.IsSuccess ? Ok() : authResult.ToProblem(authResult.Error.StatusCode);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> ConfirmEmail(EmailConfirm emailConfirm)
    {
        var result = await _authService.ConfirmEmail(emailConfirm);
        return result.IsSuccess ? Ok() : result.ToProblem(result.Error.StatusCode);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ResendConfirmEmail(ResendConfirmationEmailCodeRequest emailConfirm)
    {
        var result = await _authService.ResendEmailConfirmationCode(emailConfirm);
        return result.IsSuccess ? Ok() : result.ToProblem(result.Error.StatusCode);
    }
    [HttpPost("[action]")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.IsSuccess ? Ok() : result.ToProblem(result.Error.StatusCode);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ForgetUserPassword([FromQuery] ForgetPasswordRequest request)
    {
        var result = await _authService.ForgetPasswordAsync(User.GetUserId()!, request);
        return result.IsSuccess ? NoContent() : result.ToProblem(result.Error.StatusCode);
    }

    [HttpGet("[action]")]
    [AllowAnonymous]
    [EnableRateLimiting("tokenBucketLimit")]
    public IActionResult Test()
    {
        Thread.Sleep(10000);
        return Ok();
    }
}
