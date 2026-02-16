namespace SkillCraft.Api.Core.Lineages;

public record Weight
{
  public Roll? Malnutrition { get; }
  public Roll? Skinny { get; }
  public Roll? Normal { get; }
  public Roll? Overweight { get; }
  public Roll? Obese { get; }

  public Weight(string? malnutrition = null, string? skinny = null, string? normal = null, string? overweight = null, string? obese = null)
    : this(Roll.TryCreate(malnutrition), Roll.TryCreate(skinny), Roll.TryCreate(normal), Roll.TryCreate(overweight), Roll.TryCreate(obese))
  {
  }

  public Weight(Roll? malnutrition = null, Roll? skinny = null, Roll? normal = null, Roll? overweight = null, Roll? obese = null)
  {
    Malnutrition = malnutrition;
    Skinny = skinny;
    Normal = normal;
    Overweight = overweight;
    Obese = obese;
  }
}
