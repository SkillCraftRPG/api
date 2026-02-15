using FluentValidation;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Core.Customizations.Validators;

internal class CreateOrReplaceCustomizationValidator : AbstractValidator<CreateOrReplaceCustomizationPayload>
{
  public CreateOrReplaceCustomizationValidator()
  {
    RuleFor(x => x.Kind).IsInEnum();

    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
