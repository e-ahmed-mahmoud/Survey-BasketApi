using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Abstractions.Const;
using SurveyBasket.Authentication;
using SurveyBasket.Authentication.Authorization;
using SurveyBasket.Extensions.Emails;
using SurveyBasket.OpenApi;
using SurveyBasket.Services.Dashboard;
using SurveyBasket.Services.NotificaitonServices;
using SurveyBasket.Services.RoleServices;
using SurveyBasket.Services.UserServices;
using SurveyBasket.Services.VoteService;
using SurveyBasket.SwaggerConfig;
using Swashbuckle.AspNetCore.SwaggerGen;

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
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:61933" };
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
        services.AddHttpContextAccessor();
        services.AddHangfireConfig(configuration);
        services.ConfigRateLimit();
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            options.ReportApiVersions = true;
        }
        ).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";   // define API version format in swagger, 'v' is prefix, V is version number
            options.SubstituteApiVersionInUrl = true;   // substitute API version in URL when generating swagger docs
        });

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerAndOpenApi()
            .AddOpenApi();

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
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            options.CustomSchemaIds(type => type.FullName);

            options.OperationFilter<SwaggerDefaultValues>();

        });
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();
        return services;
    }

    private static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        //add mutiple OpenAPI versions
        var serviceProvider = services.BuildServiceProvider();
        var apiVersionDescriptionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {

            services.AddOpenApi(description.GroupName, options =>
            {

                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "Survey Basket API",
                        Version = description.ApiVersion.ToString(),
                        Description = $"An API for managing surveys and polls. Version {(description.IsDeprecated ? "This API version has been deprecated." : string.Empty)}",
                        Contact = new()
                        {
                            Name = "Ahmed Mahmoud",
                            Email = "ahmed.mahmoud.6618@gmail.com"
                        }
                    };
                    return Task.CompletedTask;
                });
            });
        }

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

        services.AddAuthorization(options =>
            options.AddPolicy("OpenAPIAdminPolicy", p => p.RequireRole(DefaultRoles.Admin)));

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
    public static IServiceCollection ConfigRateLimit(this IServiceCollection services)
    {
        services.AddRateLimiter(rateLimiterOpitons =>
        {
            rateLimiterOpitons.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiterOpitons.AddConcurrencyLimiter(policyName: "concurrencyLimit", options =>
            {
                options.PermitLimit = 2;       //number of concurrent requests
                options.QueueLimit = 1;         //requests that can wait in waiting queue 
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;    // queue style FIFO
            });
            // rateLimiterOpitons.AddTokenBucketLimiter(policyName: "tokenBucketLimit", options =>
            // {
            //     options.TokenLimit = 2;              //number of tokens in the bucket
            //     options.QueueLimit = 1;                //requests that can wait in waiting queue
            //     options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;    // queue style FIFO
            //     options.ReplenishmentPeriod = TimeSpan.FromSeconds(100);   //time to refill the bucket
            //     options.TokensPerPeriod = 1;         //number of tokens to add each replen
            //     options.AutoReplenishment = true;       //auto replenish tokens at added
            // });
            // rateLimiterOpitons.AddFixedWindowLimiter(policyName: "fixedWindowLimit", options =>
            // {
            //     options.PermitLimit = 2;              //number of requests allowed in the window
            //     options.Window = TimeSpan.FromSeconds(10);   //time window duration
            //     options.QueueLimit = 1;                //requests that can wait in waiting queue
            //     options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;    // queue style FIFO
            // });
            // rateLimiterOpitons.AddSlidingWindowLimiter(policyName: "slidingWindowLimit", options =>
            // {
            //     options.PermitLimit = 2;              //number of requests allowed in the window
            //     options.SegmentsPerWindow = 2;        //number of segments in the window
            //     options.Window = TimeSpan.FromSeconds(10);   //time window duration
            //     options.QueueLimit = 1;                //requests that can wait in waiting queue
            //     options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;    // queue style FIFO
            // });
            //Limit IP
            rateLimiterOpitons.AddPolicy("IPPolicyLimit", httpContext =>

                RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(10)
                        }
                    )
                );
            rateLimiterOpitons.AddPolicy("userLimit", httpContext =>

                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User?.Identity?.Name!.ToString() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(10)
                    }
                )
            );

        });
        return services;
    }
}
