using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.GameDb;

internal static class StorageSummary
{
  public static readonly TableId Table = new(GameContext.Schema, nameof(GameContext.StorageSummary), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(StorageSummaryEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(StorageSummaryEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(StorageSummaryEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(StorageSummaryEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(StorageSummaryEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(StorageSummaryEntity.Version), Table);

  public static readonly ColumnId AllocatedBytes = new(nameof(StorageSummaryEntity.AllocatedBytes), Table);
  public static readonly ColumnId RemainingBytes = new(nameof(StorageSummaryEntity.RemainingBytes), Table);
  public static readonly ColumnId UsedBytes = new(nameof(StorageSummaryEntity.UsedBytes), Table);
  public static readonly ColumnId WorldId = new(nameof(StorageSummaryEntity.WorldId), Table);
  public static readonly ColumnId WorldUid = new(nameof(StorageSummaryEntity.WorldUid), Table);
}
