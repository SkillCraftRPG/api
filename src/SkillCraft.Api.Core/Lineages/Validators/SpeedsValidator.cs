using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class SpeedsValidator : AbstractValidator<ISpeeds>
{
  public SpeedsValidator()
  {
    When(x => x.Walk.HasValue, () => RuleFor(x => x.Walk!.Value).GreaterThan(0));
    When(x => x.Climb.HasValue, () => RuleFor(x => x.Climb!.Value).GreaterThan(0));
    When(x => x.Swim.HasValue, () => RuleFor(x => x.Swim!.Value).GreaterThan(0));
    When(x => x.Fly.HasValue, () => RuleFor(x => x.Fly!.Value).GreaterThan(0));
    When(x => x.Hover, () => RuleFor(x => x.Fly).NotNull());
    When(x => x.Burrow.HasValue, () => RuleFor(x => x.Burrow!.Value).GreaterThan(0));
  }
}
