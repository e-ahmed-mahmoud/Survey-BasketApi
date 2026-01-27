using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

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
        return Ok(_mapper.Map<IEnumerable<PollResponse>>(await _pollService.GetAllPollsAsync(cancellationToken)));
    }


    [HttpGet("[action]/{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var poll = await _pollService.GetByIdAsync(id, cancellationToken);
        if (poll is null)
            return NotFound();

        var pollResponse = _mapper.Map<PollResponse>(poll);
        return Ok(pollResponse);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AddPoll([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var newPoll = (await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken)).Adapt<PollResponse>();

        return CreatedAtAction(nameof(GetById), routeValues: new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> UpdatePoll([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {

        var isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);

        return !isUpdated ? NotFound() : NoContent();
    }


    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeletePoll([FromRoute] int id, CancellationToken cancellationToken) => await _pollService.DeleteAsync(id, cancellationToken)
    ? NoContent() : NotFound();

    [HttpPut("[action]/{id}/togglePublished")]
    public async Task<IActionResult> TogglePublishedStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.TogglePublishedStatus(id, cancellationToken);

        return !isUpdated ? NotFound() : NoContent();
    }
}
