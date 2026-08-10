using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Worlds
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Worlds), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(WorldEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(WorldEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(WorldEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(WorldEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(WorldEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(WorldEntity.Version), Table);

  public static readonly ColumnId Content = new(nameof(WorldEntity.Content), Table);
  public static readonly ColumnId Id = new(nameof(WorldEntity.Id), Table);
  public static readonly ColumnId Key = new(nameof(WorldEntity.Key), Table);
  public static readonly ColumnId Name = new(nameof(WorldEntity.Name), Table);
  public static readonly ColumnId OwnerId = new(nameof(WorldEntity.OwnerId), Table);
  public static readonly ColumnId WorldId = new(nameof(WorldEntity.WorldId), Table);
}
