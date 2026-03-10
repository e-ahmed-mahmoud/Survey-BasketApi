using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Services;

public interface IQuestionService
{
    Task<Result<PaginatedList<QuestionResponse>>> GetAll(int pollId, RequestFilters requestFilters, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<QuestionResponse>>> GetPollQuestionsAsync(int pollId, string userId, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> GetById(int pollId, int questionId, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(int pollId, int QuestionId, QuestionRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(int pollId, int QuestionId, CancellationToken cancellationToken = default);

}
