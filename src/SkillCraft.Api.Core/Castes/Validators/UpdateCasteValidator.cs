using FluentValidation;
using SkillCraft.Api.Contracts.Castes;

namespace SkillCraft.Api.Core.Castes.Validators;

internal class UpdateCasteValidator : AbstractValidator<UpdateCastePayload>
{
  public UpdateCasteValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
