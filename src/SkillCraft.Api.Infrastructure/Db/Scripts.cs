using Logitar.Data;
using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Scripts
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Scripts), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Script.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Script.CreatedOn), Table);
  public static readonly ColumnId Description = new(nameof(Script.Description), Table);
  public static readonly ColumnId Id = new(nameof(Script.Id), Table);
  public static readonly ColumnId Name = new(nameof(Script.Name), Table);
  public static readonly ColumnId ScriptId = new(nameof(Script.ScriptId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Script.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Script.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Script.Version), Table);
  public static readonly ColumnId WorldId = new(nameof(Script.WorldId), Table);
}
