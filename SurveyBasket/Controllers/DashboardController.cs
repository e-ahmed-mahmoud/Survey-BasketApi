using Asp.Versioning;
using SurveyBasket.Services.Dashboard;

namespace SurveyBasket.Controllers;

[ApiController]
[Route("api/Polls/{pollId}/[controller]")]
[Authorize(Roles = "Admin,Member")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    private readonly IDashboardService _dashboardService = dashboardService;

    [HttpGet("[action]")]
    public async Task<IActionResult> GetPollVotes([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetPollVotesAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetPollVotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetPollVotesPerDayAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
    }
    [HttpGet("[action]")]
    public async Task<IActionResult> GetPollVotesPerAnswer([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetPollVotesPerAnswerAsync(pollId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
    }
}
