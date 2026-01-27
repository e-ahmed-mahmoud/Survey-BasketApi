
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SurveyBasket.Services.UserServices;

namespace SurveyBasket.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUserServices userServices) : IdentityDbContext(options)
{
    private readonly IUserServices _userServices = userServices;

    public DbSet<Poll> Polls { get; set; }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Poll>();

        foreach (var entity in entries)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Property(p => p.CreatedById).CurrentValue = _userServices.GetCurrentUserId()!;
            }
            if (entity.State == EntityState.Modified)
            {
                entity.Property(p => p.UpdatedById).CurrentValue = _userServices.GetCurrentUserId()!;
                entity.Property(p => p.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //apply constraints on OnModelCreating direct
        //modelBuilder.Entity<Poll>().HasIndex( p => p.Title ).IsUnique();
        //apply constraints using configuration class 
        //modelBuilder.ApplyConfiguration<Poll>(new PollEntityConfiguration());
        //or apply all configurations from the current assembly 
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

}
