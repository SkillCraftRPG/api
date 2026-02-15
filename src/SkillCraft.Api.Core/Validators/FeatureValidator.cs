using FluentValidation;
using SkillCraft.Api.Contracts;

namespace SkillCraft.Api.Core.Validators;

internal class FeatureValidator : AbstractValidator<FeatureModel>
{
  public FeatureValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
