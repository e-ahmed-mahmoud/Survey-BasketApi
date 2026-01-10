namespace SurveyBasket.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> _polls = [new() { Id = 1, Title = "poll1", Descripation = "test poll" }];

    public IEnumerable<Poll> GetAllPolls() => _polls;
    public Poll? GetById(int id) => _polls.SingleOrDefault(p => p.Id == id);
    public Poll Add(Poll poll)
    {
        poll.Id= _polls.Count + 1;
        _polls.Add(poll);
        return poll;
    }

    public bool Update(int id,Poll poll)
    {
        var pollEntity= GetById(id);
        if (pollEntity is null)
            return false;

        pollEntity.Title = poll.Title;
        pollEntity.Descripation = poll.Descripation;

        return true;
    }

    public bool Delete(int id)
    {
        var poll = GetById( id);
        if (poll is null)
            return false;

        _polls.Remove(poll);

        return true;
    }

}
