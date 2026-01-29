using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SurveyBasket.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var user = httpContext.User?.Identity?.IsAuthenticated == true ? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : "Anonymous";
            _logger.LogError(ex, "An unhandled exception occurred {exceptionMessage} from {user}", ex.Message, user ?? "Anonymous");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problemDetials = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message
            };
            await httpContext.Response.WriteAsJsonAsync(problemDetials);
        }
    }
}
