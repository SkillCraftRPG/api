using FluentValidation;
using SkillCraft.Api.Contracts.Specializations;

namespace SkillCraft.Api.Core.Specializations.Validators;

internal class CreateOrReplaceSpecializationValidator : AbstractValidator<CreateOrReplaceSpecializationPayload>
{
  public CreateOrReplaceSpecializationValidator()
  {
    RuleFor(x => x.Tier).InclusiveBetween(1, Tier.MaximumValue);

    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Summary), () => RuleFor(x => x.Summary!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    // TODO(fpion): Requirements { Talent, Other }
    // TODO(fpion): Options { Talents, Other }
    // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }
  }
}
