using Hangfire;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;
using SurveyBasket;
using SurveyBasket.Health;
using SurveyBasket.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddHealthChecks()
    .AddSqlServer(name: "DbCheck", connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddHangfire(options => { options.MinimumAvailableServers = 1; })
    .AddCheck<MailProviderHealthCheck>(name: "Mail Provider Check");
//.AddUrlGroup(name: "external URI Check ", uri: new Uri("https://zwww.google.com/"));

var app = builder.Build();

// Seed the database
await app.SeedDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();//.RequireAuthorization("OpenAPIAdminPolicy");
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    });
    app.MapScalarApiReference();
}
else
{
    app.MapOpenApi().RequireAuthorization("OpenAPIAdminPolicy");
}

app.UseRequestLocalization(new RequestLocalizationOptions()
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US")
});

app.UseHttpDataLog();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
//app.UseOutputCache();
//app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseExceptionHandler();//("error-handling-endpoint");

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [ new HangfireCustomBasicAuthenticationFilter
    {
        User = app.Configuration.GetValue<string>("HangfireSettings:User"),
        Pass = app.Configuration.GetValue<string>("HangfireSettings:Pass")
    }],
    DashboardTitle = "Survay Basket Jops",
    //IsReadOnlyFunc = (DashboardContext context) => true  //make trigger and delete dashboard buttons hidden 
});

app.UseHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.UseRateLimiter();

app.MapIdentityApi<ApplicationUser>();

app.InvokeAutomatedHangfireJobs();

app.Run();
