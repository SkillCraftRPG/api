using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterAppearanceModel
{
  public int? Height { get; set; }
  public int? Weight { get; set; }
  public int? Age { get; set; }

  public string? Skin { get; set; }
  public string? Eyes { get; set; }
  public string? Hair { get; set; }
}

internal class CharacterAppearanceValidator : AbstractValidator<CharacterAppearanceModel>
{
  public CharacterAppearanceValidator()
  {
    RuleFor(x => x.Height).InclusiveBetween(1, 999);
    RuleFor(x => x.Weight).InclusiveBetween(1, 9999);
    RuleFor(x => x.Age).InclusiveBetween(1, 9999);

    When(x => !string.IsNullOrWhiteSpace(x.Skin), () => RuleFor(x => x.Skin!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Eyes), () => RuleFor(x => x.Eyes!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Hair), () => RuleFor(x => x.Hair!).Name());
  }
}
