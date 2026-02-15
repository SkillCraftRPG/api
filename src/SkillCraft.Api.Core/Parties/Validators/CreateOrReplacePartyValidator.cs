using FluentValidation;
using SkillCraft.Api.Contracts.Parties;

namespace SkillCraft.Api.Core.Parties.Validators;

internal class CreateOrReplacePartyValidator : AbstractValidator<CreateOrReplacePartyPayload>
{
  public CreateOrReplacePartyValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
