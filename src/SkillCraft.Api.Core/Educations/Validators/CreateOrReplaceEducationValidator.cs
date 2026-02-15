using FluentValidation;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core.Validators;

namespace SkillCraft.Api.Core.Educations.Validators;

internal class CreateOrReplaceEducationValidator : AbstractValidator<CreateOrReplaceEducationPayload>
{
  public CreateOrReplaceEducationValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Skill).IsInEnum();
    When(x => x.WealthMultiplier is not null, () => RuleFor(x => x.WealthMultiplier!.Value).WealthMultiplier());
    When(x => x.Feature is not null, () => RuleFor(x => x.Feature!).SetValidator(new FeatureValidator()));
  }
}
