using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class Lineages
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.Lineages), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(LineageEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(LineageEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(LineageEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(LineageEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(LineageEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(LineageEntity.Version), Table);

  public static readonly ColumnId LineageId = new(nameof(LineageEntity.LineageId), Table);
  public static readonly ColumnId Description = new(nameof(LineageEntity.Description), Table);
  public static readonly ColumnId Id = new(nameof(LineageEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(LineageEntity.Name), Table);
  public static readonly ColumnId Summary = new(nameof(LineageEntity.Summary), Table);
  public static readonly ColumnId ParentId = new(nameof(LineageEntity.ParentId), Table);
  public static readonly ColumnId ParentUid = new(nameof(LineageEntity.ParentUid), Table);
  public static readonly ColumnId WorldId = new(nameof(LineageEntity.WorldId), Table);
  public static readonly ColumnId WorldUid = new(nameof(LineageEntity.WorldUid), Table);
}
