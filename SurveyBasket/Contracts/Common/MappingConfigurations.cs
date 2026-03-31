using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Contracts;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollResponse>().Map(dest => dest.Summary, src => src.Summary);
        config.NewConfig<PollRequest, Poll>();
        //config.NewConfig<QuestionRequest, Question>().Ignore(x => x.Answers);
        config.NewConfig<QuestionRequest, Question>().
        Map(dest => dest.Answers, src => src.Answers.Select(a => new Answer { Content = a }).ToList(), src => src.Answers != null);

        config.NewConfig<Question, QuestionResponse>()
            .Map(dest => dest.Answers, src => src.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content)));

        config.NewConfig<RegisterRequest, ApplicationUser>().Map(dist => dist.UserName, src => src.Email);

        config.NewConfig<ApplicationUser, UserProfile>()
            .Map(dest => dest.FullName, src => String.Concat(src.FirstName, " ", src.LastName))
            .Map(dist => dist.UserName, src => src.FirstName);

        config.NewConfig<(ApplicationUser, IList<string>), UserResponse>().Map(dest => dest, user => user.Item1)
            .Map(dest => dest.Roles, src => src.Item2);
    }
}
