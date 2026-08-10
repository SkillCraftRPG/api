using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Castes
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Castes), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(CasteEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(CasteEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(CasteEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(CasteEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(CasteEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(CasteEntity.Version), Table);

  public static readonly ColumnId CasteId = new(nameof(CasteEntity.CasteId), Table);
  public static readonly ColumnId Content = new(nameof(CasteEntity.Content), Table);
  public static readonly ColumnId FeatureContent = new(nameof(CasteEntity.FeatureContent), Table);
  public static readonly ColumnId FeatureName = new(nameof(CasteEntity.FeatureName), Table);
  public static readonly ColumnId Id = new(nameof(CasteEntity.Id), Table);
  public static readonly ColumnId Name = new(nameof(CasteEntity.Name), Table);
  public static readonly ColumnId Skill = new(nameof(CasteEntity.Skill), Table);
  public static readonly ColumnId Summary = new(nameof(CasteEntity.Summary), Table);
  public static readonly ColumnId WealthRoll = new(nameof(CasteEntity.WealthRoll), Table);
  public static readonly ColumnId WorldId = new(nameof(CasteEntity.WorldId), Table);
}
