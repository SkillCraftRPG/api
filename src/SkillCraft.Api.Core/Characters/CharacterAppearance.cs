using FluentValidation;
using Logitar;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterAppearance
{
  int? Height { get; }
  int? Weight { get; }
  int? Age { get; }

  string? Skin { get; }
  string? Eyes { get; }
  string? Hair { get; }
}

public record CharacterAppearance : ICharacterAppearance
{
  public const int MaximumLength = 20;

  public int? Height { get; }
  public int? Weight { get; }
  public int? Age { get; }

  public string? Skin { get; }
  public string? Eyes { get; }
  public string? Hair { get; }

  public CharacterAppearance()
  {
  }

  [JsonConstructor]
  public CharacterAppearance(int? height, int? weight, int? age, string? skin, string? eyes, string? hair)
  {
    Height = height;
    Weight = weight;
    Age = age;

    Skin = skin?.CleanTrim();
    Eyes = eyes?.CleanTrim();
    Hair = hair?.CleanTrim();

    new CharacterAppearanceValidator().ValidateAndThrow(this);
  }

  public CharacterAppearance(ICharacterAppearance appearance)
    : this(appearance.Height, appearance.Weight, appearance.Age, appearance.Skin, appearance.Eyes, appearance.Hair)
  {
  }
}

internal class CharacterAppearanceValidator : AbstractValidator<ICharacterAppearance>
{
  public CharacterAppearanceValidator()
  {
    RuleFor(x => x.Height).InclusiveBetween(1, 9999);
    RuleFor(x => x.Weight).InclusiveBetween(1, 9999);
    RuleFor(x => x.Age).InclusiveBetween(1, 9999);

    When(x => !string.IsNullOrWhiteSpace(x.Skin), () => RuleFor(x => x.Skin).NotEmpty().MaximumLength(CharacterAppearance.MaximumLength));
    When(x => !string.IsNullOrWhiteSpace(x.Eyes), () => RuleFor(x => x.Eyes).NotEmpty().MaximumLength(CharacterAppearance.MaximumLength));
    When(x => !string.IsNullOrWhiteSpace(x.Hair), () => RuleFor(x => x.Hair).NotEmpty().MaximumLength(CharacterAppearance.MaximumLength));
  }
}
