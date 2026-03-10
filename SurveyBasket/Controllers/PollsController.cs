
using Asp.Versioning;
using SurveyBasket.Abstractions.Const;
using SurveyBasket.Authentication.Authorization;

namespace SurveyBasket.Controllers;

[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [MapToApiVersion(1.0)]
    [HttpGet("GetPolls")]
    [HasPermission(policyName: PermissionsClaims.GetPolls)]
    public async Task<IActionResult> GetPollsV1(CancellationToken cancellationToken)
    {
        return Ok((await _pollService.GetAllPollsAsync(cancellationToken)).Value);
    }

    [MapToApiVersion("2.0")]
    [HttpGet("GetPolls")]
    [HasPermission(policyName: PermissionsClaims.GetPolls)]
    public async Task<IActionResult> GetPollsV2(CancellationToken cancellationToken) => Ok((await _pollService.GetAllPollsV2Async(cancellationToken)).Value);

    [HttpGet("[action]")]
    public async Task<IActionResult> GetCurrentPolls(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetCurrentPollsAsync(cancellationToken);
        return Ok(polls);
    }

    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) :
        result.ToProblem(result.Error.StatusCode);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AddPoll([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(request, cancellationToken);
        return result.IsSuccess ?
            CreatedAtAction(nameof(GetById), routeValues: new { id = result.Value.Id }, result.Value)
            : result.ToProblem(StatusCodes.Status409Conflict);
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> UpdatePoll([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {

        var result = await _pollService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess ?
        NoContent() :
        result.ToProblem(result.Error.StatusCode);
    }


    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeletePoll([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ?
            NoContent() :
            result.ToProblem(result.Error.StatusCode);
    }

    [HttpPut("[action]/{id}/togglePublished")]
    public async Task<IActionResult> TogglePublishedStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishedStatus(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem(result.Error.StatusCode);
    }
}
