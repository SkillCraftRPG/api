using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class CreateOrReplaceLineageValidator : AbstractValidator<CreateOrReplaceLineagePayload>
{
  public CreateOrReplaceLineageValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Speeds).SetValidator(new SpeedsValidator());
    RuleFor(x => x.Size).SetValidator(new SizeValidator());
  }
}
