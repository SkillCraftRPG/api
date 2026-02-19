using FluentValidation;
using SkillCraft.Api.Contracts.Characters;

namespace SkillCraft.Api.Core.Characters.Validators;

internal class CharacteristicsValidator : AbstractValidator<ICharacteristics>
{
  public CharacteristicsValidator()
  {
    When(x => x.Height.HasValue, () => RuleFor(x => x.Height!.Value).GreaterThan(0));
    When(x => x.Weight.HasValue, () => RuleFor(x => x.Weight!.Value).GreaterThan(0));
    When(x => x.Age.HasValue, () => RuleFor(x => x.Age!.Value).GreaterThan(0));
    When(x => !string.IsNullOrWhiteSpace(x.Skin), () => RuleFor(x => x.Skin!).Characteristic());
    When(x => !string.IsNullOrWhiteSpace(x.Eyes), () => RuleFor(x => x.Eyes!).Characteristic());
    When(x => !string.IsNullOrWhiteSpace(x.Hair), () => RuleFor(x => x.Hair!).Characteristic());
    RuleFor(x => x.Handedness).IsInEnum();
  }
}
