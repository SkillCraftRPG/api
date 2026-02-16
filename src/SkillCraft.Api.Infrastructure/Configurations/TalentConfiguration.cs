using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class TalentConfiguration : AggregateConfiguration<TalentEntity>, IEntityTypeConfiguration<TalentEntity>
{
  public override void Configure(EntityTypeBuilder<TalentEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Talents), GameContext.Schema);
    builder.HasKey(x => x.TalentId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.WorldUid);
    builder.HasIndex(x => x.Tier);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Summary);
    builder.HasIndex(x => x.AllowMultiplePurchases);
    builder.HasIndex(x => x.Skill);
    builder.HasIndex(x => x.RequiredTalentId);
    builder.HasIndex(x => x.RequiredTalentUid);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);
    builder.Property(x => x.Skill).HasMaxLength(16).HasConversion(new EnumToStringConverter<GameSkill>());

    builder.HasOne(x => x.World).WithMany(x => x.Talents).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(x => x.RequiredTalent).WithMany(x => x.RequiringTalents)
      .HasPrincipalKey(x => x.TalentId).HasForeignKey(x => x.RequiredTalentId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
