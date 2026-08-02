using Logitar.Data;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Lineages
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Lineages), alias: null);

  public static readonly ColumnId Adult = new(nameof(LineageEntity.Adult), Table);
  public static readonly ColumnId Burrow = new(nameof(LineageEntity.Burrow), Table);
  public static readonly ColumnId Climb = new(nameof(LineageEntity.Climb), Table);
  public static readonly ColumnId Content = new(nameof(LineageEntity.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(LineageEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(LineageEntity.CreatedOn), Table);
  public static readonly ColumnId CustomNames = new(nameof(LineageEntity.CustomNames), Table);
  public static readonly ColumnId ExtraLanguages = new(nameof(LineageEntity.ExtraLanguages), Table);
  public static readonly ColumnId FamilyNames = new(nameof(LineageEntity.FamilyNames), Table);
  public static readonly ColumnId FemaleNames = new(nameof(LineageEntity.FemaleNames), Table);
  public static readonly ColumnId Fly = new(nameof(LineageEntity.Fly), Table);
  public static readonly ColumnId HeightRoll = new(nameof(LineageEntity.HeightRoll), Table);
  public static readonly ColumnId Hover = new(nameof(LineageEntity.Hover), Table);
  public static readonly ColumnId Id = new(nameof(LineageEntity.Id), Table);
  public static readonly ColumnId LanguagesContent = new(nameof(LineageEntity.LanguagesContent), Table);
  public static readonly ColumnId LineageId = new(nameof(LineageEntity.LineageId), Table);
  public static readonly ColumnId MaleNames = new(nameof(LineageEntity.MaleNames), Table);
  public static readonly ColumnId Malnutrition = new(nameof(LineageEntity.Malnutrition), Table);
  public static readonly ColumnId Mature = new(nameof(LineageEntity.Mature), Table);
  public static readonly ColumnId Name = new(nameof(LineageEntity.Name), Table);
  public static readonly ColumnId NamesContent = new(nameof(LineageEntity.NamesContent), Table);
  public static readonly ColumnId NormalWeight = new(nameof(LineageEntity.NormalWeight), Table);
  public static readonly ColumnId Obese = new(nameof(LineageEntity.Obese), Table);
  public static readonly ColumnId Overweight = new(nameof(LineageEntity.Overweight), Table);
  public static readonly ColumnId ParentId = new(nameof(LineageEntity.ParentId), Table);
  public static readonly ColumnId SizeCategory = new(nameof(LineageEntity.SizeCategory), Table);
  public static readonly ColumnId Skinny = new(nameof(LineageEntity.Skinny), Table);
  public static readonly ColumnId Summary = new(nameof(LineageEntity.Summary), Table);
  public static readonly ColumnId Swim = new(nameof(LineageEntity.Swim), Table);
  public static readonly ColumnId Teenager = new(nameof(LineageEntity.Teenager), Table);
  public static readonly ColumnId UnisexNames = new(nameof(LineageEntity.UnisexNames), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(LineageEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(LineageEntity.UpdatedOn), Table);
  public static readonly ColumnId Venerable = new(nameof(LineageEntity.Venerable), Table);
  public static readonly ColumnId Version = new(nameof(LineageEntity.Version), Table);
  public static readonly ColumnId Walk = new(nameof(LineageEntity.Walk), Table);
  public static readonly ColumnId WorldId = new(nameof(LineageEntity.WorldId), Table);
}
