using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class UpdateLineageValidator : AbstractValidator<UpdateLineagePayload>
{
  public UpdateLineageValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => x.Speeds is not null, () => RuleFor(x => x.Speeds!).SetValidator(new SpeedsValidator()));
    When(x => x.Size is not null, () => RuleFor(x => x.Size!).SetValidator(new SizeValidator()));
  }
}
