using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class AgeValidator : AbstractValidator<IAge>
{
  public AgeValidator()
  {
    RuleFor(x => x.Teenager).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Adult).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Mature).GreaterThanOrEqualTo(0);
    RuleFor(x => x.Venerable).GreaterThanOrEqualTo(0);
  }
}
