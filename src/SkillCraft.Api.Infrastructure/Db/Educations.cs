using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Educations
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Educations), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(EducationEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(EducationEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(EducationEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(EducationEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(EducationEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(EducationEntity.Version), Table);

  public static readonly ColumnId Content = new(nameof(EducationEntity.Content), Table);
  public static readonly ColumnId EducationId = new(nameof(EducationEntity.EducationId), Table);
  public static readonly ColumnId FeatureContent = new(nameof(EducationEntity.FeatureContent), Table);
  public static readonly ColumnId FeatureName = new(nameof(EducationEntity.FeatureName), Table);
  public static readonly ColumnId Id = new(nameof(EducationEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(EducationEntity.Name), Table);
  public static readonly ColumnId Skill = new(nameof(EducationEntity.Skill), Table);
  public static readonly ColumnId Summary = new(nameof(EducationEntity.Summary), Table);
  public static readonly ColumnId WealthMultiplier = new(nameof(EducationEntity.WealthMultiplier), Table);
  public static readonly ColumnId WorldId = new(nameof(EducationEntity.WorldId), Table);
}
