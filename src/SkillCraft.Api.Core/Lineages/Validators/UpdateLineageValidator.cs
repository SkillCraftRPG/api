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

    // TODO(fpion): Features
    When(x => x.Languages is not null, () => RuleFor(x => x.Languages!).SetValidator(new LanguagesValidator()));
    When(x => x.Names is not null, () => RuleFor(x => x.Names!).SetValidator(new NamesValidator()));

    When(x => x.Speeds is not null, () => RuleFor(x => x.Speeds!).SetValidator(new SpeedsValidator()));
    When(x => x.Size is not null, () => RuleFor(x => x.Size!).SetValidator(new SizeValidator()));
    When(x => x.Weight is not null, () => RuleFor(x => x.Weight!).SetValidator(new WeightValidator()));
    When(x => x.Age is not null, () => RuleFor(x => x.Age!).SetValidator(new AgeValidator()));
  }
}
