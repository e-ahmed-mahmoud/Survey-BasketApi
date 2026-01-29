namespace SurveyBasket.Services;


public class PollService(ApplicationDbContext dbContext) : IPollService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    public async Task<Result<List<PollResponse>>> GetAllPollsAsync(CancellationToken cancellationToken = default)
    {
        var polls = await _dbContext.Polls.AsNoTracking().ToListAsync(cancellationToken);
        return polls is null || !polls.Any() ?
            Result.Failure<List<PollResponse>>(PollErrors.NotDefinedError) :
            Result.Success(polls.Adapt<List<PollResponse>>());
    }
    public async Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _dbContext.Polls.FindAsync(id, cancellationToken);
        return poll is null ?
            Result.Failure<PollResponse>(PollErrors.NotDefinedError) :
            Result.Success(poll.Adapt<PollResponse>());
    }
    public async Task<Result<PollResponse>> AddAsync(PollRequest pollRequest, CancellationToken cancellationToken = default)
    {
        var isTitleExsits = await _dbContext.Polls.AnyAsync(p => p.Title == pollRequest.Title, cancellationToken);
        if (isTitleExsits)
            return Result.Failure<PollResponse>(PollErrors.DuplicateTitleError);

        var poll = pollRequest.Adapt<Poll>();
        await _dbContext.Polls.AddAsync(poll, cancellationToken);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0 ? Result.Success<PollResponse>(poll.Adapt<PollResponse>()) :
            Result.Failure<PollResponse>(PollErrors.InvalidPollDataError);
    }

    public async Task<Result> UpdateAsync(int id, PollRequest pollRequest, CancellationToken cancellationToken = default)
    {

        var isTitleExsits = await _dbContext.Polls.AnyAsync(p => p.Title == pollRequest.Title && p.Id != id, cancellationToken);
        if (isTitleExsits)
            return Result.Failure<PollResponse>(PollErrors.DuplicateTitleError);

        var poll = pollRequest.Adapt<Poll>();
        var entity = await _dbContext.Polls.FindAsync(id, cancellationToken);

        if (entity is null)
            return Result.Failure(PollErrors.NotDefinedError);

        entity.Title = poll.Title;
        entity.Summary = poll.Summary;
        entity.IsPublished = poll.IsPublished;
        entity.StartAt = poll.StartAt;
        entity.EndAt = poll.EndAt;

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _dbContext.Polls.FindAsync(id, cancellationToken);
        if (poll is null)
            return Result.Failure(PollErrors.NotDefinedError);
        _dbContext.Polls.Remove(poll);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> TogglePublishedStatus(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _dbContext.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.NotPublishedError);
        poll.IsPublished = !poll.IsPublished;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
