using FluentValidation;
using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Core.Talents.Validators;

internal class CreateOrReplaceTalentValidator : AbstractValidator<CreateOrReplaceTalentPayload>
{
  public CreateOrReplaceTalentValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
