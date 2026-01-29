using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;

namespace SurveyBasket.Errors;

public class GlobalExceptionsHandler (ILogger<GlobalExceptionsHandler> logger): IExceptionHandler
{
    private readonly ILogger<GlobalExceptionsHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
            var user = httpContext.User?.Identity?.IsAuthenticated == true ? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : "Anonymous";
            _logger.LogError(exception, "An unhandled exception occurred {exceptionMessage} from {user}", exception.Message, user ?? "Anonymous");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problemDetials = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception.Message
            };
            await httpContext.Response.WriteAsJsonAsync(problemDetials);
            return true;
    }    
}
