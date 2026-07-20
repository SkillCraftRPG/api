using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Validation;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class TalentConfiguration : IEntityTypeConfiguration<Talent>
{
  public void Configure(EntityTypeBuilder<Talent> builder)
  {
    builder.ToTable(nameof(GameContext.Talents), Schemas.Game);
    builder.HasKey(x => x.TalentId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Tier });
    builder.HasIndex(x => new { x.WorldId, x.Name });
    builder.HasIndex(x => new { x.WorldId, x.Summary });
    builder.HasIndex(x => new { x.WorldId, x.AllowMultiplePurchases });
    builder.HasIndex(x => new { x.WorldId, x.Skill });
    builder.HasIndex(x => new { x.WorldId, x.RequiredTalentId });
    builder.HasIndex(x => new { x.WorldId, x.Version });
    builder.HasIndex(x => new { x.WorldId, x.CreatedBy });
    builder.HasIndex(x => new { x.WorldId, x.CreatedOn });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedBy });
    builder.HasIndex(x => new { x.WorldId, x.UpdatedOn });

    builder.Property(x => x.Name).HasMaxLength(Constants.NameMaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Constants.SummaryMaximumLength);
    builder.Property(x => x.Skill).HasMaxLength(16).HasConversion(new EnumToStringConverter<Skill>());

    builder.HasOne(x => x.World).WithMany(x => x.Talents)
      .HasForeignKey(x => x.WorldId).HasPrincipalKey(x => x.Id)
      .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.RequiredTalent).WithMany(x => x.RequiringTalents)
      .HasForeignKey(x => x.RequiredTalentId).HasPrincipalKey(x => x.TalentId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
