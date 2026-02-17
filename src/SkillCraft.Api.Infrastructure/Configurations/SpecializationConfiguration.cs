using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class SpecializationConfiguration : AggregateConfiguration<SpecializationEntity>, IEntityTypeConfiguration<SpecializationEntity>
{
  public override void Configure(EntityTypeBuilder<SpecializationEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(GameContext.Specializations), GameContext.Schema);
    builder.HasKey(x => x.SpecializationId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => x.WorldUid);
    builder.HasIndex(x => x.Tier);
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Summary);

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);
    builder.Property(x => x.Summary).HasMaxLength(Summary.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Specializations).OnDelete(DeleteBehavior.Restrict);
  }
}
