
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        // Add services to the container.

        services.AddControllers();

        services.AddMapSterConfig();
        services.AddFluentValidationConfig();
        services.AddSwaggerAndOpenApi();

        services.AddScoped<IPollService, PollService>();
        return services;
    }

    public static IServiceCollection AddSwaggerAndOpenApi(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        return services;
    }

    public static IServiceCollection AddMapSterConfig(this IServiceCollection services)
    {
        // Add Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(config));

        return services;
    }
    public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
