using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class LineageLanguages
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.LineageLanguages), alias: null);

  public static readonly ColumnId LanguageId = new(nameof(LineageLanguageEntity.LanguageId), Table);
  public static readonly ColumnId LineageId = new(nameof(LineageLanguageEntity.LineageId), Table);
}
