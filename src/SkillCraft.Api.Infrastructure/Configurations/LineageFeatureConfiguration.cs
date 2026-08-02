using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageFeatureConfiguration : IEntityTypeConfiguration<LineageFeatureEntity>
{
  public void Configure(EntityTypeBuilder<LineageFeatureEntity> builder)
  {
    builder.ToTable(nameof(GameContext.LineageFeatures), Schemas.Game);
    builder.HasKey(x => x.LineageFeatureId);

    builder.HasIndex(x => new { x.LineageId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.LineageId, x.Name });
    builder.HasIndex(x => new { x.LineageId, x.CreatedBy });
    builder.HasIndex(x => new { x.LineageId, x.CreatedOn });
    builder.HasIndex(x => new { x.LineageId, x.UpdatedBy });
    builder.HasIndex(x => new { x.LineageId, x.UpdatedOn });

    builder.Property(x => x.Name).HasMaxLength(Name.MaximumLength);

    builder.HasOne(x => x.Lineage).WithMany(x => x.Features)
      .HasForeignKey(x => x.LineageId).HasPrincipalKey(x => x.LineageId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
