using FluentValidation;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Core.Customizations.Validators;

internal class UpdateCustomizationValidator : AbstractValidator<UpdateCustomizationPayload>
{
  public UpdateCustomizationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
