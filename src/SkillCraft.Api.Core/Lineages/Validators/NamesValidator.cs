using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class NamesValidator : AbstractValidator<NamesModel>
{
  public NamesValidator()
  {
    RuleForEach(x => x.Custom).SetValidator(new NameCategoryValidator());
  }
}
