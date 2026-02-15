using FluentValidation;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Core.Validators;

namespace SkillCraft.Api.Core.Educations.Validators;

internal class UpdateEducationValidator : AbstractValidator<UpdateEducationPayload>
{
  public UpdateEducationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Skill is not null, () => RuleFor(x => x.Skill!.Value).IsInEnum());
    When(x => x.WealthMultiplier?.Value is not null, () => RuleFor(x => x.WealthMultiplier!.Value!).WealthMultiplier());
    When(x => x.Feature?.Value is not null, () => RuleFor(x => x.Feature!.Value!).SetValidator(new FeatureValidator()));
  }
}
