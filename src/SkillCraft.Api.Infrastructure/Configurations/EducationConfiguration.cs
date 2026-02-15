using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class EducationConfiguration : AggregateConfiguration<EducationEntity>, IEntityTypeConfiguration<EducationEntity>
{
  public override void Configure(EntityTypeBuilder<EducationEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Educations), GameContext.Schema);
    builder.HasKey(x => x.EducationId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.WorldUid);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Summary);
    builder.HasIndex(x => x.Skill);
    builder.HasIndex(x => x.FeatureName);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);
    builder.Property(x => x.Skill).HasMaxLength(16).HasConversion(new EnumToStringConverter<GameSkill>());
    builder.Property(x => x.FeatureName).HasMaxLength(Name.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Educations).OnDelete(DeleteBehavior.Restrict);
  }
}
