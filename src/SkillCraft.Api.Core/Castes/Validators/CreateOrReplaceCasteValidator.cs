using FluentValidation;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core.Validators;

namespace SkillCraft.Api.Core.Castes.Validators;

internal class CreateOrReplaceCasteValidator : AbstractValidator<CreateOrReplaceCastePayload>
{
  public CreateOrReplaceCasteValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Skill).IsInEnum();
    When(x => !string.IsNullOrWhiteSpace(x.WealthRoll), () => RuleFor(x => x.WealthRoll!).Roll());
    When(x => x.Feature is not null, () => RuleFor(x => x.Feature!).SetValidator(new FeatureValidator()));
  }
}
