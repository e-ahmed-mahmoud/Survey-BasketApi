using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.Persistence.EntitiesConfigurations;

public class VoteAnswerEntityConfig : IEntityTypeConfiguration<VoteAnswer>
{
    public void Configure(EntityTypeBuilder<VoteAnswer> builder)
    {
        builder.HasIndex(v => new { v.VoteId, v.QuestionId }).IsUnique();
    }
}
