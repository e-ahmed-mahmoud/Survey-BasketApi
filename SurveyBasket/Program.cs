using Scalar.AspNetCore;
using SurveyBasket;
using SurveyBasket.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
    app.MapScalarApiReference();
}

app.UseHttpDataLog();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
