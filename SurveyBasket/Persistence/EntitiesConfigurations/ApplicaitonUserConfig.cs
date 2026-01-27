using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Persistence.EntitiesConfigurations;

public class ApplicaitonUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(50).IsRequired();

        builder.OwnsMany(e => e.RefreshTokens).ToTable(name: "RefreshTokens").WithOwner().HasForeignKey("UserId");
    }
}
