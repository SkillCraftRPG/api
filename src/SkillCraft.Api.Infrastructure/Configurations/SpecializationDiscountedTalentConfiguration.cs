using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class SpecializationDiscountedTalentConfiguration : IEntityTypeConfiguration<SpecializationDiscountedTalentEntity>
{
  public void Configure(EntityTypeBuilder<SpecializationDiscountedTalentEntity> builder)
  {
    builder.ToTable(nameof(GameContext.SpecializationDiscountedTalents), GameContext.Schema);
    builder.HasKey(x => new { x.SpecializationId, x.TalentId });

    builder.HasIndex(x => x.SpecializationUid);
    builder.HasIndex(x => x.TalentUid);

    builder.HasOne(x => x.Specialization).WithMany(x => x.DiscountedTalents).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Talent).WithMany(x => x.SpecializationDiscounts).OnDelete(DeleteBehavior.Cascade);
  }
}
