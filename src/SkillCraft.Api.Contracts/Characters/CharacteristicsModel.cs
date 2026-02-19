namespace SkillCraft.Api.Contracts.Characters;

public record CharacteristicsModel : ICharacteristics
{
  public int? Height { get; set; }
  public int? Weight { get; set; }
  public int? Age { get; set; }
  public string? Skin { get; set; }
  public string? Eyes { get; set; }
  public string? Hair { get; set; }
  public Handedness? Handedness { get; set; }

  public CharacteristicsModel(int? height = null, int? weight = null, int? age = null, string? skin = null, string? eyes = null, string? hair = null, Handedness? handedness = null)
  {
    Height = height;
    Weight = weight;
    Age = age;
    Skin = skin;
    Eyes = eyes;
    Hair = hair;
    Handedness = handedness;
  }

  public CharacteristicsModel(ICharacteristics characteristics)
    : this(characteristics.Height, characteristics.Weight, characteristics.Age, characteristics.Skin, characteristics.Eyes, characteristics.Hair, characteristics.Handedness)
  {
  }
}
