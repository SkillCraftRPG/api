using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class SpeedsValidator : AbstractValidator<ISpeeds>
{
  public SpeedsValidator()
  {
    RuleFor(x => x.Walk).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Climb).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Swim).GreaterThanOrEqualTo(0);
    When(x => x.Hover, () => RuleFor(x => x.Fly).GreaterThan(0)).Otherwise(() => RuleFor(x => x.Fly).GreaterThanOrEqualTo(0));
    RuleFor(x => x.Burrow).GreaterThanOrEqualTo(0);
  }
}
