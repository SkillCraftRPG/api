using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class Castes
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.Castes), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(CasteEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CasteEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(CasteEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CasteEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CasteEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(CasteEntity.Version), Table);

  public static readonly ColumnId CasteId = new(nameof(CasteEntity.CasteId), Table);
  public static readonly ColumnId Description = new(nameof(CasteEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(CasteEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(CasteEntity.Name), Table);
  public static readonly ColumnId Summary = new(nameof(CasteEntity.Summary), Table);
  public static readonly ColumnId WorldId = new(nameof(CasteEntity.WorldId), Table);
  public static readonly ColumnId WorldUid = new(nameof(CasteEntity.WorldUid), Table);
}
