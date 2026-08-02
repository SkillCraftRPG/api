using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillCraft.Api.Infrastructure.Db;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Configurations;

internal class LineageLanguageConfiguration : IEntityTypeConfiguration<LineageLanguageEntity>
{
  public void Configure(EntityTypeBuilder<LineageLanguageEntity> builder)
  {
    builder.ToTable(nameof(GameContext.LineageLanguages), Schemas.Game);
    builder.HasKey(x => new { x.LineageId, x.LanguageId });
  }
}
