using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Authentication;
using SurveyBasket.Services.UserServices;

namespace SurveyBasket;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        //add DbContent
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
        });
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
        services.AddCors(options => options.AddPolicy("AllowAll",
            builder => builder.WithOrigins(allowedOrigins!).AllowAnyMethod().AllowAnyHeader())); //WithOrigins("http://localhost:3000") for specific origins

        // Add services to the container.
        services.AddControllers();
        services.AddIdentityConfig(configuration);
        services.AddMapsterConfig();
        services.AddFluentValidationConfig();
        services.AddSwaggerAndOpenApi();

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddScoped<IUserServices, UserServices>();
        return services;
    }

    private static IServiceCollection AddSwaggerAndOpenApi(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        // Add Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(config));

        return services;
    }
    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
    private static IServiceCollection AddIdentityConfig(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddApiEndpoints()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddSingleton<IJwtProvider, JwtProvider>();

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName)
        .ValidateDataAnnotations().ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        //configure JWT Bearer token generation
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // saves token after successful authorization, if false, the token will encrypt and decrypt each time each time
            options.SaveToken = true;
            // configure validation parameters, object take which parameters to validate during authorization, also provide values to validate against
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                ClockSkew = TimeSpan.Zero // remove delay of token when expire
            };
        });

        return services;
    }
}
