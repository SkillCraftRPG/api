using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class LineageLanguages
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.LineageLanguages), alias: null);

  public static readonly ColumnId LanguageId = new(nameof(LineageLanguageEntity.LanguageId), Table);
  public static readonly ColumnId LanguageUid = new(nameof(LineageLanguageEntity.LanguageUid), Table);
  public static readonly ColumnId LineageId = new(nameof(LineageLanguageEntity.LineageId), Table);
  public static readonly ColumnId LineageUid = new(nameof(LineageLanguageEntity.LineageUid), Table);
}
