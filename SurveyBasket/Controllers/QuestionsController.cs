
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;

namespace SurveyBasket.Controllers;

[ApiController]
[Route("api/Polls/{pollId}/[controller]")]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet("[action]")]
    //[AllowAnonymous]
    //[OutputCache(PolicyName = "PollsOutputCache")]
    public async Task<IActionResult> GetAll([FromRoute] int pollId, [FromQuery] RequestFilters filters, CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAll(pollId, filters, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(result.Error.StatusCode);
    }

    [HttpGet("[action]/{questionId}")]
    public async Task<IActionResult> GetQuestionById([FromRoute] int pollId, [FromRoute] int questionId, CancellationToken cancellationToken)
    {
        var res = await _questionService.GetById(pollId, questionId, cancellationToken);
        return res.IsSuccess ? Ok(res.Value) : res.ToProblem(res.Error.StatusCode);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AddQuestion([FromRoute] int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionService.AddAsync(pollId, request, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem(result.Error.StatusCode);
    }

    [HttpPut("[action]/{questionId}")]
    public async Task<IActionResult> UpdateQuestion([FromRoute] int pollId, [FromRoute] int questionId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionService.UpdateAsync(pollId, questionId, request, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem(result.Error.StatusCode);
    }

    [HttpDelete("[action]/{questionId}")]
    public async Task<IActionResult> DeleteQuestion([FromRoute] int pollId, [FromRoute] int questionId, CancellationToken cancellationToken)
    {
        var result = await _questionService.DeleteAsync(pollId, questionId, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem(result.Error.StatusCode);
    }

}