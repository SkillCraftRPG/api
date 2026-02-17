using FluentValidation;
using SkillCraft.Api.Contracts.Specializations;

namespace SkillCraft.Api.Core.Specializations.Validators;

internal class UpdateSpecializationValidator : AbstractValidator<UpdateSpecializationPayload>
{
  public UpdateSpecializationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Summary?.Value), () => RuleFor(x => x.Summary!.Value!).Summary());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    // TODO(fpion): Requirements { Talent, Other }
    // TODO(fpion): Options { Talents, Other }
    // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }
  }
}
