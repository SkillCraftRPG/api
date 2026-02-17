using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class LanguagesValidator : AbstractValidator<LanguagesPayload>
{
  public LanguagesValidator()
  {
    RuleFor(x => x.Extra).GreaterThanOrEqualTo(0);
  }
}
