using Logitar.Data;
using SkillCraft.Api.Core.Castes;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Castes
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Castes), alias: null);

  public static readonly ColumnId CasteId = new(nameof(Caste.CasteId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(Caste.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Caste.CreatedOn), Table);
  public static readonly ColumnId FeatureHtmlContent = new(nameof(Caste.FeatureHtmlContent), Table);
  public static readonly ColumnId FeatureName = new(nameof(Caste.FeatureName), Table);
  public static readonly ColumnId HtmlContent = new(nameof(Caste.HtmlContent), Table);
  public static readonly ColumnId Id = new(nameof(Caste.Id), Table);
  public static readonly ColumnId Name = new(nameof(Caste.Name), Table);
  public static readonly ColumnId Skill = new(nameof(Caste.Skill), Table);
  public static readonly ColumnId Summary = new(nameof(Caste.Summary), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Caste.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Caste.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Caste.Version), Table);
  public static readonly ColumnId WealthRoll = new(nameof(Caste.WealthRoll), Table);
  public static readonly ColumnId WorldId = new(nameof(Caste.WorldId), Table);
}
