
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SurveyBasket.Extensions;
using SurveyBasket.Persistence.EntitiesConfigurations;
using SurveyBasket.Services.UserServices;

namespace SurveyBasket.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public DbSet<Answer> Answers { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Poll> Polls { get; set; }

    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteAnswer> VoteAnswers { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditLogging>();

        foreach (var entity in entries)
        {
            var currentUserId = _httpContextAccessor?.HttpContext?.User.GetUserId();
            if (entity.State == EntityState.Added)
            {
                entity.Property(p => p.CreatedById).CurrentValue = currentUserId ?? "48606D7C-F8DA-406D-B9B6-C1107E14A515";
            }
            if (entity.State == EntityState.Modified)
            {
                entity.Property(p => p.UpdatedById).CurrentValue = currentUserId!;
                entity.Property(p => p.UpdatedOn).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //apply gloabl restrict on delete behavior for all Fks
        var cascadesFk = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys())
        .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var fk in cascadesFk)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        //apply constraints on OnModelCreating direct
        //modelBuilder.Entity<Poll>().HasIndex( p => p.Title ).IsUnique();
        //apply constraints using configuration class 
        //modelBuilder.ApplyConfiguration<Poll>(new PollEntityConfiguration());
        //or apply all configurations from the current assembly 
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

}
