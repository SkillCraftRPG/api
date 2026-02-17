namespace SkillCraft.Api.Core.Lineages;

public record Weight
{
  public Roll? Malnutrition { get; }
  public Roll? Skinny { get; }
  public Roll? Normal { get; }
  public Roll? Overweight { get; }
  public Roll? Obese { get; }

  [JsonIgnore]
  public long Size => (Malnutrition?.Size ?? 0) + (Skinny?.Size ?? 0) + (Normal?.Size ?? 0) + (Overweight?.Size ?? 0) + (Obese?.Size ?? 0);

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

  public static Weight Create(string? malnutrition, string? skinny, string? normal, string? overweight, string? obese)
    => new(Roll.TryCreate(malnutrition), Roll.TryCreate(skinny), Roll.TryCreate(normal), Roll.TryCreate(overweight), Roll.TryCreate(obese));
}
