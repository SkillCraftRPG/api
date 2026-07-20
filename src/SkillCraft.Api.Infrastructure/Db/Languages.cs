using Logitar.Data;
using SkillCraft.Api.Core.Languages;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Languages
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Languages), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Language.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Language.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(Language.HtmlContent), Table);
  public static readonly ColumnId Id = new(nameof(Language.Id), Table);
  public static readonly ColumnId LanguageId = new(nameof(Language.LanguageId), Table);
  public static readonly ColumnId Name = new(nameof(Language.Name), Table);
  public static readonly ColumnId ScriptId = new(nameof(Language.ScriptId), Table);
  public static readonly ColumnId Summary = new(nameof(Language.Summary), Table);
  public static readonly ColumnId TypicalSpeakers = new(nameof(Language.TypicalSpeakers), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Language.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Language.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Language.Version), Table);
  public static readonly ColumnId WorldId = new(nameof(Language.WorldId), Table);
}
