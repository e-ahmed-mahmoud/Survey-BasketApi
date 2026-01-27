
namespace SurveyBasket.Services;


public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;


    public async Task<IEnumerable<Poll>> GetAllPollsAsync(CancellationToken cancellationToken = default) => await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
    public async Task<Poll?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => await _context.Polls.FindAsync(id, cancellationToken);
    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        await _context.Polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Polls.FindAsync(id, cancellationToken);

        if (entity is null)
            return false;

        entity.Title = poll.Title;
        entity.Summary = poll.Summary;
        entity.IsPublished = poll.IsPublished;
        entity.StartAt = poll.StartAt;
        entity.EndAt = poll.EndAt;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
        if (poll is null)
            return false;

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> TogglePublishedStatus(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return false;
        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
