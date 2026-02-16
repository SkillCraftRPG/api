using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class WeightValidator : AbstractValidator<WeightModel>
{
  public WeightValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Malnutrition), () => RuleFor(x => x.Malnutrition!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Skinny), () => RuleFor(x => x.Skinny!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Normal), () => RuleFor(x => x.Normal!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Overweight), () => RuleFor(x => x.Overweight!).Roll());
    When(x => !string.IsNullOrWhiteSpace(x.Obese), () => RuleFor(x => x.Obese!).Roll());
  }
}
