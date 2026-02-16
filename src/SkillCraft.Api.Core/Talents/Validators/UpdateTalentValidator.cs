using FluentValidation;
using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Core.Talents.Validators;

internal class UpdateTalentValidator : AbstractValidator<UpdateTalentPayload>
{
  public UpdateTalentValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Skill is not null, () => RuleFor(x => x.Skill!.Value).IsInEnum());
  }
}
