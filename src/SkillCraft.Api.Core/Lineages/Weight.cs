namespace SkillCraft.Api.Core.Lineages;

public record Weight
{
  public Roll? Malnutrition { get; }
  public Roll? Skinny { get; }
  public Roll? Normal { get; }
  public Roll? Overweight { get; }
  public Roll? Obese { get; }

  public Weight()
  {
  }

  [JsonConstructor]
  public Weight(Roll? malnutrition, Roll? skinny, Roll? normal, Roll? overweight, Roll? obese)
  {
    Malnutrition = malnutrition;
    Skinny = skinny;
    Normal = normal;
    Overweight = overweight;
    Obese = obese;
  }
}
