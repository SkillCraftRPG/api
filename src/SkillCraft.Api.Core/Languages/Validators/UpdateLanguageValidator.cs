using FluentValidation;
using SkillCraft.Api.Contracts.Languages;

namespace SkillCraft.Api.Core.Languages.Validators;

internal class UpdateLanguageValidator : AbstractValidator<UpdateLanguagePayload>
{
  public UpdateLanguageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.TypicalSpeakers?.Value), () => RuleFor(x => x.TypicalSpeakers!.Value!).TypicalSpeakers());
  }
}
