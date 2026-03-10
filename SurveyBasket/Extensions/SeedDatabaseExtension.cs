namespace SurveyBasket.Extensions;

public static class SeedDatabaseExtension
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed the database
            await SurveyBasket.Persistence.SeedData.SeedDatabaseAsync(context);
        }
    }
}
