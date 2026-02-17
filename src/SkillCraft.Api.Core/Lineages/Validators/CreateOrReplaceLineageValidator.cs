using FluentValidation;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Validators;

namespace SkillCraft.Api.Core.Lineages.Validators;

internal class CreateOrReplaceLineageValidator : AbstractValidator<CreateOrReplaceLineagePayload>
{
  public CreateOrReplaceLineageValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleForEach(x => x.Features).SetValidator(new FeatureValidator());
    RuleFor(x => x.Languages).SetValidator(new LanguagesValidator());
    RuleFor(x => x.Names).SetValidator(new NamesValidator());

    RuleFor(x => x.Speeds).SetValidator(new SpeedsValidator());
    RuleFor(x => x.Size).SetValidator(new SizeValidator());
    RuleFor(x => x.Weight).SetValidator(new WeightValidator());
    RuleFor(x => x.Age).SetValidator(new AgeValidator());
  }
}
