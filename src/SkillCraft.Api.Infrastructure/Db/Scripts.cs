using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Scripts
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Scripts), alias: null);

  public static readonly ColumnId Content = new(nameof(ScriptEntity.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(ScriptEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ScriptEntity.CreatedOn), Table);
  public static readonly ColumnId Id = new(nameof(ScriptEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(ScriptEntity.Name), Table);
  public static readonly ColumnId ScriptId = new(nameof(ScriptEntity.ScriptId), Table);
  public static readonly ColumnId Summary = new(nameof(ScriptEntity.Summary), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ScriptEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ScriptEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ScriptEntity.Version), Table);
  public static readonly ColumnId WorldId = new(nameof(ScriptEntity.WorldId), Table);
}
