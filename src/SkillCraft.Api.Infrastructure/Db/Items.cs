using Logitar.Data;
using SkillCraft.Api.Core.Items;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Items
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Items), alias: null);

  public static readonly ColumnId Content = new(nameof(Item.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(Item.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Item.CreatedOn), Table);
  public static readonly ColumnId Id = new(nameof(Item.Id), Table);
  public static readonly ColumnId ItemId = new(nameof(Item.ItemId), Table);
  public static readonly ColumnId Name = new(nameof(Item.Name), Table);
  public static readonly ColumnId Price = new(nameof(Item.Price), Table);
  public static readonly ColumnId Summary = new(nameof(Item.Summary), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Item.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Item.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Item.Version), Table);
  public static readonly ColumnId Weight = new(nameof(Item.Weight), Table);
  public static readonly ColumnId WorldId = new(nameof(Item.WorldId), Table);
}
