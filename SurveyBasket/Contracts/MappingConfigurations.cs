
using Mapster;

namespace SurveyBasket.Contracts;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Poll, PollResponse>().Map(dest => dest.Desc, src => src.Descripation);
        config.NewConfig<CreatePollRequest, Poll>();

    }
}
