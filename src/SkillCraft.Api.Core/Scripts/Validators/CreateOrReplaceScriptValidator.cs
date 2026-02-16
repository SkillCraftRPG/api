using FluentValidation;
using SkillCraft.Api.Contracts.Scripts;

namespace SkillCraft.Api.Core.Scripts.Validators;

internal class CreateOrReplaceScriptValidator : AbstractValidator<CreateOrReplaceScriptPayload>
{
  public CreateOrReplaceScriptValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
