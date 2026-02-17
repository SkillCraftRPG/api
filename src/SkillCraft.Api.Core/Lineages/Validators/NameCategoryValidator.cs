using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class NameCategoryValidator : AbstractValidator<NameCategory>
{
  public NameCategoryValidator()
  {
    RuleFor(x => x.Category).NotEmpty(); // TODO(fpion): maximum length
  }
}
