using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Validation;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageFeatureConfiguration : IEntityTypeConfiguration<LineageFeature>
{
  public void Configure(EntityTypeBuilder<LineageFeature> builder)
  {
    builder.ToTable(nameof(GameContext.LineageFeatures), Schemas.Game);
    builder.HasKey(x => x.LineageFeatureId);

    builder.HasIndex(x => new { x.LineageId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.LineageId, x.Name });
    builder.HasIndex(x => new { x.LineageId, x.CreatedBy });
    builder.HasIndex(x => new { x.LineageId, x.CreatedOn });
    builder.HasIndex(x => new { x.LineageId, x.UpdatedBy });
    builder.HasIndex(x => new { x.LineageId, x.UpdatedOn });

    builder.Property(x => x.Name).HasMaxLength(Constants.NameMaximumLength);

    builder.HasOne(x => x.Lineage).WithMany(x => x.Features)
      .HasForeignKey(x => x.LineageId).HasPrincipalKey(x => x.LineageId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
