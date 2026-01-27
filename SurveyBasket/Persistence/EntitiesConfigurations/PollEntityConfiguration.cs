using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Persistence.EntitiesConfigurations;

public class PollEntityConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure (EntityTypeBuilder<Poll> builder)
    {
        builder.HasIndex(p => p.Title).IsUnique();
        builder.Property(p => p.Title).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Summary).HasMaxLength(1500);
    }
}
