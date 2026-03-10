namespace SurveyBasket.Services;

public interface IPollService
{
    Task<Result<List<PollResponse>>> GetAllPollsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PollResponse>> GetCurrentPollsAsync(CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> AddAsync(PollRequest poll, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<Result> TogglePublishedStatus(int id, CancellationToken cancellationToken = default);
    Task<Result<List<PollResponseV2>>> GetAllPollsV2Async(CancellationToken cancellationToken = default);
}
