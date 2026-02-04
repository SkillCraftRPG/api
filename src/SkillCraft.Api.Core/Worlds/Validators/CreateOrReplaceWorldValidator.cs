using FluentValidation;
using SkillCraft.Api.Contracts.Worlds;

namespace SkillCraft.Api.Core.Worlds.Validators;

internal class CreateOrReplaceWorldValidator : AbstractValidator<CreateOrReplaceWorldPayload>
{
  public CreateOrReplaceWorldValidator()
  {
    RuleFor(x => x.Name).Name();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
  }
}
