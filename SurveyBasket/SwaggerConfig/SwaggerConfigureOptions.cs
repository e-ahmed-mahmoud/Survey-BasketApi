using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SurveyBasket.SwaggerConfig;

public class SwaggerConfigureOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            //iterate on each controller and get verions and data from it and add it to swagger doc
            options.SwaggerDoc(description.GroupName, ConfigureInfoSwaggerDoc(description));
        }
        //adding authentication and authorization to swagger
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
        });
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            {
                // Pass the 'document' variable as the second argument
                new OpenApiSecuritySchemeReference("Bearer", document),
                new List<string>()
            }
        });
    }
    private static OpenApiInfo ConfigureInfoSwaggerDoc(ApiVersionDescription description) => new()
    {
        Title = "Survey Basket API",
        Version = description.ApiVersion.ToString(),
        Description = $"An API for managing surveys and polls. Version {(description.IsDeprecated ? "This API version has been deprecated." : string.Empty)}",
        Contact = new OpenApiContact
        {
            Name = "Ahmed Mahmoud",
            Email = "ahmed.mahmoud.6618@gmail.com"
        }
    };

}
