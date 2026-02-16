using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageLanguageConfiguration : IEntityTypeConfiguration<LineageLanguageEntity>
{
  public void Configure(EntityTypeBuilder<LineageLanguageEntity> builder)
  {
    builder.ToTable(nameof(GameContext.LineageLanguages), GameContext.Schema);
    builder.HasKey(x => new { x.LineageId, x.LanguageId });

    builder.HasIndex(x => x.LineageUid);
    builder.HasIndex(x => x.LanguageUid);

    builder.HasOne(x => x.Lineage).WithMany(x => x.Languages).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Language).WithMany(x => x.Lineages).OnDelete(DeleteBehavior.Cascade);
  }
}
