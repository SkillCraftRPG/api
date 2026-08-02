using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class LineageFeatures
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.LineageFeatures), alias: null);

  public static readonly ColumnId Content = new(nameof(LineageFeatureEntity.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(LineageFeatureEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(LineageFeatureEntity.CreatedOn), Table);
  public static readonly ColumnId Id = new(nameof(LineageFeatureEntity.Id), Table);
  public static readonly ColumnId LineageFeatureId = new(nameof(LineageFeatureEntity.LineageFeatureId), Table);
  public static readonly ColumnId LineageId = new(nameof(LineageFeatureEntity.LineageId), Table);
  public static readonly ColumnId Name = new(nameof(LineageFeatureEntity.Name), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(LineageFeatureEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(LineageFeatureEntity.UpdatedOn), Table);
}
