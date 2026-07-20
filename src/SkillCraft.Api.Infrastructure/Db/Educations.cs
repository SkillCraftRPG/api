using Logitar.Data;
using SkillCraft.Api.Core.Educations;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Educations
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Educations), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(Education.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Education.CreatedOn), Table);
  public static readonly ColumnId EducationId = new(nameof(Education.EducationId), Table);
  public static readonly ColumnId FeatureHtmlContent = new(nameof(Education.FeatureHtmlContent), Table);
  public static readonly ColumnId FeatureName = new(nameof(Education.FeatureName), Table);
  public static readonly ColumnId HtmlContent = new(nameof(Education.HtmlContent), Table);
  public static readonly ColumnId Id = new(nameof(Education.Id), Table);
  public static readonly ColumnId Name = new(nameof(Education.Name), Table);
  public static readonly ColumnId Skill = new(nameof(Education.Skill), Table);
  public static readonly ColumnId Summary = new(nameof(Education.Summary), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Education.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Education.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Education.Version), Table);
  public static readonly ColumnId WealthMultiplier = new(nameof(Education.WealthMultiplier), Table);
  public static readonly ColumnId WorldId = new(nameof(Education.WorldId), Table);
}
