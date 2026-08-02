using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Customizations
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Customizations), alias: null);

  public static readonly ColumnId Content = new(nameof(CustomizationEntity.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(CustomizationEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CustomizationEntity.CreatedOn), Table);
  public static readonly ColumnId CustomizationId = new(nameof(CustomizationEntity.CustomizationId), Table);
  public static readonly ColumnId Id = new(nameof(CustomizationEntity.Id), Table);
  public static readonly ColumnId Kind = new(nameof(CustomizationEntity.Kind), Table);
  public static readonly ColumnId Name = new(nameof(CustomizationEntity.Name), Table);
  public static readonly ColumnId Summary = new(nameof(CustomizationEntity.Summary), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CustomizationEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CustomizationEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(CustomizationEntity.Version), Table);
  public static readonly ColumnId WorldId = new(nameof(CustomizationEntity.WorldId), Table);
}
