using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageLanguage
{
  public Lineage? Lineage { get; set; }
  public int LineageId { get; set; }

  public LanguageEntity? Language { get; set; }
  public int LanguageId { get; set; }
}

internal class LineageLanguageConfiguration : IEntityTypeConfiguration<LineageLanguage>
{
  public void Configure(EntityTypeBuilder<LineageLanguage> builder)
  {
    builder.ToTable(nameof(GameContext.LineageLanguages), Schemas.Game);
    builder.HasKey(x => new { x.LineageId, x.LanguageId });

    builder.HasOne(x => x.Lineage).WithMany().HasForeignKey(x => x.LineageId);
    builder.HasOne(x => x.Language).WithMany().HasForeignKey(x => x.LanguageId);
  }
}
