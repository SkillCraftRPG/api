using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class SpecializationOptionalTalentConfiguration : IEntityTypeConfiguration<SpecializationOptionalTalentEntity>
{
  public void Configure(EntityTypeBuilder<SpecializationOptionalTalentEntity> builder)
  {
    builder.ToTable(nameof(GameContext.SpecializationOptionalTalents), GameContext.Schema);
    builder.HasKey(x => new { x.SpecializationId, x.TalentId });

    builder.HasIndex(x => x.SpecializationUid);
    builder.HasIndex(x => x.TalentUid);

    builder.HasOne(x => x.Specialization).WithMany(x => x.OptionalTalents).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Talent).WithMany(x => x.SpecializationOptions).OnDelete(DeleteBehavior.Cascade);
  }
}
