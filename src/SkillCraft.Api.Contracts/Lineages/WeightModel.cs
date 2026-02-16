namespace SkillCraft.Api.Contracts.Lineages;

public record WeightModel
{
  public string? Malnutrition { get; set; }
  public string? Skinny { get; set; }
  public string? Normal { get; set; }
  public string? Overweight { get; set; }
  public string? Obese { get; set; }

  public WeightModel(string? malnutrition = null, string? skinny = null, string? normal = null, string? overweight = null, string? obese = null)
  {
    Malnutrition = malnutrition;
    Skinny = skinny;
    Normal = normal;
    Overweight = overweight;
    Obese = obese;
  }
}
