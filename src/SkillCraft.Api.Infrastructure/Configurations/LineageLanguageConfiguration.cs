using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageLanguage
{
  public int LineageId { get; set; }
  public int LanguageId { get; set; }
}

internal class LineageLanguageConfiguration : IEntityTypeConfiguration<LineageLanguage>
{
  public void Configure(EntityTypeBuilder<LineageLanguage> builder)
  {
    builder.ToTable(nameof(GameContext.LineageLanguages), Schemas.Game);
    builder.HasKey(x => new { x.LineageId, x.LanguageId });
  }
}
