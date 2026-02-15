using FluentValidation;
using SkillCraft.Api.Contracts.Parties;

namespace SkillCraft.Api.Core.Parties.Validators;

internal class UpdatePartyValidator : AbstractValidator<UpdatePartyPayload>
{
  public UpdatePartyValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
