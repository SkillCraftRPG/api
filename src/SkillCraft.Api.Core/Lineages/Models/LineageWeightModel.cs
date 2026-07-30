namespace SkillCraft.Api.Core.Lineages.Models;

public record LineageWeightModel
{
  public string? Malnutrition { get; set; }
  public string? Skinny { get; set; }
  public string? Normal { get; set; }
  public string? Overweight { get; set; }
  public string? Obese { get; set; }
}
