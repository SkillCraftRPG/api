using FluentValidation;
using SkillCraft.Api.Contracts.Characters;
using SkillCraft.Api.Core.Characters.Validators;

namespace SkillCraft.Api.Core.Characters;

public record Characteristics : ICharacteristics
{
  public const int MaximumLength = 20;

  public int? Height { get; }
  public int? Weight { get; }
  public int? Age { get; }
  public string? Skin { get; }
  public string? Eyes { get; }
  public string? Hair { get; }
  public Handedness? Handedness { get; }

  [JsonIgnore]
  public long Size => (Skin?.Length ?? 0) + (Eyes?.Length ?? 0) + (Hair?.Length ?? 0);

  public Characteristics()
  {
  }

  public Characteristics(ICharacteristics characteristics)
    : this(characteristics.Height, characteristics.Weight, characteristics.Age, characteristics.Skin, characteristics.Eyes, characteristics.Hair, characteristics.Handedness)
  {
  }

  [JsonConstructor]
  public Characteristics(int? height, int? weight, int? age, string? skin, string? eyes, string? hair, Handedness? handedness)
  {
    Height = height;
    Weight = weight;
    Age = age;
    Skin = skin;
    Eyes = eyes;
    Hair = hair;
    Handedness = handedness;
    new CharacteristicsValidator().ValidateAndThrow(this);
  }
}
