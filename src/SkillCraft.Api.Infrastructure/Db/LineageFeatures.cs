using Logitar.Data;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Infrastructure.Db;

public static class LineageFeatures
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.LineageFeatures), alias: null);

  public static readonly ColumnId Content = new(nameof(LineageFeature.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(LineageFeature.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(LineageFeature.CreatedOn), Table);
  public static readonly ColumnId Id = new(nameof(LineageFeature.Id), Table);
  public static readonly ColumnId LineageFeatureId = new(nameof(LineageFeature.LineageFeatureId), Table);
  public static readonly ColumnId LineageId = new(nameof(LineageFeature.LineageId), Table);
  public static readonly ColumnId Name = new(nameof(LineageFeature.Name), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(LineageFeature.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(LineageFeature.UpdatedOn), Table);
}
