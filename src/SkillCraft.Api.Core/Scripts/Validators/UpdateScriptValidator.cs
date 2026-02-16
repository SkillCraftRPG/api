using FluentValidation;
using SkillCraft.Api.Contracts.Scripts;

namespace SkillCraft.Api.Core.Scripts.Validators;

internal class UpdateScriptValidator : AbstractValidator<UpdateScriptPayload>
{
  public UpdateScriptValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
