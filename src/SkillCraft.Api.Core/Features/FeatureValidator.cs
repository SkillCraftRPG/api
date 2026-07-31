using FluentValidation;
using SkillCraft.Api.Core.Validation;

namespace SkillCraft.Api.Core.Features;

internal class FeatureValidator : AbstractValidator<IFeature>
{
  public FeatureValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Content), () => RuleFor(x => x.Content!).Content());
  }
}
