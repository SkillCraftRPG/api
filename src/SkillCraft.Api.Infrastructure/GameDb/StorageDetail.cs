using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class StorageDetail
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.StorageDetail), alias: null);

  public static readonly ColumnId EntityId = new(nameof(StorageDetailEntity.EntityId), Table);
  public static readonly ColumnId EntityKind = new(nameof(StorageDetailEntity.EntityKind), Table);
  public static readonly ColumnId Key = new(nameof(StorageDetailEntity.Key), Table);
  public static readonly ColumnId Size = new(nameof(StorageDetailEntity.Size), Table);
  public static readonly ColumnId StorageDetailId = new(nameof(StorageDetailEntity.StorageDetailId), Table);
  public static readonly ColumnId WorldId = new(nameof(StorageDetailEntity.WorldId), Table);
  public static readonly ColumnId WorldUid = new(nameof(StorageDetailEntity.WorldUid), Table);
}
