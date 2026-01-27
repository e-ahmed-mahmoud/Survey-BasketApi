namespace SurveyBasket.Contracts;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollResponse>().Map(dest => dest.Summary, src => src.Summary);
        config.NewConfig<PollRequest, Poll>();

    }
}
