using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Items
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Items), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(ItemEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ItemEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(ItemEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ItemEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ItemEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(ItemEntity.Version), Table);

  public static readonly ColumnId Category = new(nameof(ItemEntity.Category), Table);
  public static readonly ColumnId Content = new(nameof(ItemEntity.Content), Table);
  public static readonly ColumnId Id = new(nameof(ItemEntity.Id), Table);
  public static readonly ColumnId ItemId = new(nameof(ItemEntity.ItemId), Table);
  public static readonly ColumnId Name = new(nameof(ItemEntity.Name), Table);
  public static readonly ColumnId Price = new(nameof(ItemEntity.Price), Table);
  public static readonly ColumnId Properties = new(nameof(ItemEntity.Properties), Table);
  public static readonly ColumnId Summary = new(nameof(ItemEntity.Summary), Table);
  public static readonly ColumnId Weight = new(nameof(ItemEntity.Weight), Table);
  public static readonly ColumnId WorldId = new(nameof(ItemEntity.WorldId), Table);
}
