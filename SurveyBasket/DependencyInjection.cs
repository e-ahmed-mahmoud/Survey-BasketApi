using System.Reflection;
using System.Text;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Authentication;
using SurveyBasket.Authentication.Authorization;
using SurveyBasket.Extensions.Emails;
using SurveyBasket.Services.Dashboard;
using SurveyBasket.Services.NotificaitonServices;
using SurveyBasket.Services.RoleServices;
using SurveyBasket.Services.UserServices;
using SurveyBasket.Services.VoteService;

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

        services.AddHybridCache();

        //configure Email Settings
        services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));


        // Add services to the container.
        services.AddExceptionHandler<GlobalExceptionsHandler>();
        services.AddProblemDetails();
        services.AddControllers();
        services.AddIdentityConfig(configuration);
        services.AddMapsterConfig();
        services.AddFluentValidationConfig();
        services.AddSwaggerAndOpenApi();
        services.AddHttpContextAccessor();
        services.AddHangfireConfig(configuration);

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<INotificaitonService, NotificaitonService>();
        services.AddSingleton<IEmailSender, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
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

        services.AddIdentity<ApplicationUser, ApplicationRole>()
        .AddApiEndpoints()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        //IdentityConfiguration 
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
        });

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationPolicyHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
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

    public static IServiceCollection AddHangfireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

        // Add the processing server as IHostedService
        services.AddHangfireServer();
        return services;
    }
}
