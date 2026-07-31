using Logitar.Data;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Infrastructure.Db;

public static class Lineages
{
  public static readonly TableId Table = new(Schemas.Game, nameof(GameContext.Lineages), alias: null);

  public static readonly ColumnId Adult = new(nameof(Lineage.Adult), Table);
  public static readonly ColumnId Burrow = new(nameof(Lineage.Burrow), Table);
  public static readonly ColumnId Climb = new(nameof(Lineage.Climb), Table);
  public static readonly ColumnId Content = new(nameof(Lineage.Content), Table);
  public static readonly ColumnId CreatedBy = new(nameof(Lineage.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Lineage.CreatedOn), Table);
  public static readonly ColumnId CustomNames = new(nameof(Lineage.CustomNames), Table);
  public static readonly ColumnId ExtraLanguages = new(nameof(Lineage.ExtraLanguages), Table);
  public static readonly ColumnId FamilyNames = new(nameof(Lineage.FamilyNames), Table);
  public static readonly ColumnId FemaleNames = new(nameof(Lineage.FemaleNames), Table);
  public static readonly ColumnId Fly = new(nameof(Lineage.Fly), Table);
  public static readonly ColumnId HeightRoll = new(nameof(Lineage.HeightRoll), Table);
  public static readonly ColumnId Hover = new(nameof(Lineage.Hover), Table);
  public static readonly ColumnId Id = new(nameof(Lineage.Id), Table);
  public static readonly ColumnId LanguagesContent = new(nameof(Lineage.LanguagesContent), Table);
  public static readonly ColumnId LineageId = new(nameof(Lineage.LineageId), Table);
  public static readonly ColumnId MaleNames = new(nameof(Lineage.MaleNames), Table);
  public static readonly ColumnId Malnutrition = new(nameof(Lineage.Malnutrition), Table);
  public static readonly ColumnId Mature = new(nameof(Lineage.Mature), Table);
  public static readonly ColumnId Name = new(nameof(Lineage.Name), Table);
  public static readonly ColumnId NamesContent = new(nameof(Lineage.NamesContent), Table);
  public static readonly ColumnId NormalWeight = new(nameof(Lineage.NormalWeight), Table);
  public static readonly ColumnId Obese = new(nameof(Lineage.Obese), Table);
  public static readonly ColumnId Overweight = new(nameof(Lineage.Overweight), Table);
  public static readonly ColumnId ParentId = new(nameof(Lineage.ParentId), Table);
  public static readonly ColumnId SizeCategory = new(nameof(Lineage.SizeCategory), Table);
  public static readonly ColumnId Skinny = new(nameof(Lineage.Skinny), Table);
  public static readonly ColumnId Summary = new(nameof(Lineage.Summary), Table);
  public static readonly ColumnId Swim = new(nameof(Lineage.Swim), Table);
  public static readonly ColumnId Teenager = new(nameof(Lineage.Teenager), Table);
  public static readonly ColumnId UnisexNames = new(nameof(Lineage.UnisexNames), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Lineage.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Lineage.UpdatedOn), Table);
  public static readonly ColumnId Venerable = new(nameof(Lineage.Venerable), Table);
  public static readonly ColumnId Version = new(nameof(Lineage.Version), Table);
  public static readonly ColumnId Walk = new(nameof(Lineage.Walk), Table);
  public static readonly ColumnId WorldId = new(nameof(Lineage.WorldId), Table);
}
