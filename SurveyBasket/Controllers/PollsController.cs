
namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService, IMapper mapper) : ControllerBase
{
    private readonly IPollService _pollService = pollService;
    private readonly IMapper _mapper = mapper;

    [HttpGet("[action]")]
    public async Task<IActionResult> GetPolls(CancellationToken cancellationToken)
    {
        return Ok((await _pollService.GetAllPollsAsync(cancellationToken)).Value);
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
