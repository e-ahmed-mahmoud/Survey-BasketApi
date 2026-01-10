namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService, IMapper mapper) : ControllerBase
{
    private readonly IPollService _pollService = pollService;
    private readonly IMapper _mapper = mapper;

    [HttpGet("[action]")]
    public IActionResult GetPolls()
    {
        return Ok(_mapper.Map<IEnumerable<PollResponse>>(_pollService.GetAllPolls()));
    }


    [HttpGet("[action]/{id}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var poll = _pollService.GetById(id);
        if (poll is null)
            return NotFound();

        var pollResponse = _mapper.Map<PollResponse>(poll);
        return Ok(pollResponse);
    }

    [HttpPost("[action]")]
    public IActionResult AddPoll([FromBody] CreatePollRequest request)
    {
        var newPoll = _pollService.Add(request.Adapt<Poll>()).Adapt<PollResponse>();

        return CreatedAtAction(nameof(GetById), routeValues: new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("[action]/{id}")]
    public IActionResult UpdatePoll([FromRoute] int id, [FromBody] CreatePollRequest request)
    {

        var isUpdated = _pollService.Update(id, request.Adapt<Poll>());

        return !isUpdated ? NotFound() : NoContent();
    }


    [HttpDelete("[action]/{id}")]
    public IActionResult DeletePoll([FromRoute] int id) => _pollService.Delete(id) ? NoContent() : NotFound();
}
