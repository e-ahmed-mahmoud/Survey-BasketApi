using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using SurveyBasket.Contracts.Questions;
using SurveyBasket.Services.DistributedCache;

namespace SurveyBasket.Services;

public class QuestionService(ApplicationDbContext context, HybridCache hybridCache) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly HybridCache _hybridCache = hybridCache;
    private const string _cacheKeyPrefix = "PollId-";
    public async Task<Result<PaginatedList<QuestionResponse>>> GetAll(int pollId, RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExists)
            return Result.Failure<PaginatedList<QuestionResponse>>(new Error("Poll not found", "The specified poll does not exist.", 404));

        //var uniqueCachedKey = String.Concat(_cacheKeyPrefix, pollId);
        var query = _context.Questions.Where(q => q.IsActive && q.PollId == pollId);

        if (!string.IsNullOrEmpty(filters.Search))
        {
            query = query.Where(x => x.Content.Contains(filters.Search) || x.Poll.Title.Contains(filters.Search));
        }
        //.ToListAsync(cancellationToken); //, new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30) });
        if (!string.IsNullOrEmpty(filters.Sort))
        {
            //Order by constructor only define in Linq.Dyinamic 
            query = query.OrderBy($"{filters.Sort} {filters.SortDir ?? "asc"}");
        }

        var source = query.Include(q => q.Answers).ProjectToType<QuestionResponse>().AsNoTracking();
        var pages = await PaginatedList<QuestionResponse>.CreatePageAsync(source, filters.PageSize, filters.PageNumber, cancellationToken);

        return Result.Success(pages);
    }
    public async Task<Result<QuestionResponse>> GetById(int pollId, int questionId, CancellationToken cancellationToken)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId);
        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(new Error("Poll not found", "The specified poll does not exist.", 404));
        var question = await _context.Questions.Where(q => q.Id == questionId && q.PollId == pollId && q.IsActive)
                        .Include(q => q.Answers).ProjectToType<QuestionResponse>().AsNoTracking().FirstOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(new Error("Question not found", "The specified question does not exist in the given poll.", 404));

        return Result.Success(question);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken)
    {
        var isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!isPollExists)
            return Result.Failure<QuestionResponse>(new Error("Poll not found", "The specified poll does not exist.", 404));

        var isContentExists = await _context.Questions.AnyAsync(q => q.Content == request.Content);
        if (isContentExists)
            return Result.Failure<QuestionResponse>(new Error("Duplicate question", "A question with the same content already exists.", 409));

        var question = request.Adapt<Question>();
        question.PollId = pollId;
        question.Answers = [.. request.Answers.Select(ac => new Answer { Content = ac })];

        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        //await _outputCacheStore.EvictByTagAsync("pollsPolicyTag", cancellationToken);
        await _hybridCache.RemoveAsync(String.Concat(_cacheKeyPrefix, pollId), cancellationToken);
        return Result.Success(question.Adapt<QuestionResponse>());

    }

    public async Task<Result> UpdateAsync(int pollId, int QuestionId, QuestionRequest request, CancellationToken cancellationToken)
    {
        var isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!isPollExists)
            return Result.Failure(new Error("Poll not found", "The specified poll does not exist,", 404));

        var question = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == QuestionId && q.PollId == pollId && q.IsActive, cancellationToken);
        if (question is null)
            return Result.Failure(new Error("Question not found", "The specified question does not exist in the given poll.", 404));

        var isContentExists = await _context.Questions.AnyAsync(q => q.Content == request.Content && q.Id != QuestionId && q.PollId == pollId, cancellationToken);
        if (isContentExists)
            return Result.Failure(new Error("Duplicate question", "A question with the same content already exists.", 409));

        var currentAnswers = question.Answers.Select(a => a.Content).ToList();
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        if (newAnswers.Count > 0)
            newAnswers.ForEach(a => question.Answers.Add(new Answer { Content = a }));

        question.Answers.ToList().ForEach(a =>
        {
            if (!request.Answers.Contains(a.Content))
                a.IsActive = false;
        });

        question.Content = request.Content;
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync(String.Concat(_cacheKeyPrefix, pollId), cancellationToken);

        return Result.Success();
    }
    public async Task<Result> DeleteAsync(int pollId, int QuestionId, CancellationToken cancellationToken)
    {
        var isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!isPollExists)
            return Result.Failure(new Error("Poll not found", "The specified poll does not exist.", 404));

        var question = await _context.Questions.FirstOrDefaultAsync(q => q.Id == QuestionId && q.PollId == pollId && q.IsActive, cancellationToken);
        if (question is null)
            return Result.Failure(new Error("Question not found", "The specified question does not exist in the given poll.", 404));

        question.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetPollQuestionsAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished &&
        p.StartAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(new Error("Poll not found", "The specified poll does not exist.", 404));

        var isUserVotedBefore = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
        if (isUserVotedBefore)
            return Result.Failure<IEnumerable<QuestionResponse>>(new Error("User has already voted", "The user has already submitted a vote for this poll.", 409));

        var pollQuestions = await _context.Questions
        .Where(q => q.IsActive && q.PollId == pollId).Include(q => q.Answers).AsNoTracking().ProjectToType<QuestionResponse>().ToListAsync(cancellationToken);

        if (pollQuestions is null || pollQuestions.Count == 0)
            return Result.Failure<IEnumerable<QuestionResponse>>(new Error("Questions not found", "No questions found for the specified poll.", 404));

        return Result.Success<IEnumerable<QuestionResponse>>(pollQuestions);

    }
}
