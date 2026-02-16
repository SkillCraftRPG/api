namespace SkillCraft.Api.Core.Lineages;

public record Weight
{
  public Roll? Malnutrition { get; }
  public Roll? Skinny { get; }
  public Roll? Normal { get; }
  public Roll? Overweight { get; }
  public Roll? Obese { get; }

  public Weight(Roll? malnutrition = null, Roll? skinny = null, Roll? normal = null, Roll? overweight = null, Roll? obese = null)
  {
    Malnutrition = malnutrition;
    Skinny = skinny;
    Normal = normal;
    Overweight = overweight;
    Obese = obese;
  }
}
