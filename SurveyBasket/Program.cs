using Scalar.AspNetCore;
using SurveyBasket;
using SurveyBasket.Middlewares;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
var defaultLog = builder.Configuration["Logging:LogLevel:Default"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
    app.MapScalarApiReference();
}

app.UseRequestLocalization(new RequestLocalizationOptions()
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US")
});

app.UseHttpDataLog();

app.UseHttpsRedirection();

app.UseCors("AllowAll");
//app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseExceptionHandler();//("error-handling-endpoint");

app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

app.Run();
