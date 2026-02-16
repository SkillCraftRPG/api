using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class SizeValidator : AbstractValidator<SizeModel>
{
  public SizeValidator()
  {
    RuleFor(x => x.Category).IsInEnum();
    RuleFor(x => x.Height).Roll();
  }
}
