using FluentValidation;
using SkillCraft.Api.Contracts.Worlds;

namespace SkillCraft.Api.Core.Worlds.Validators;

internal class UpdateWorldValidator : AbstractValidator<UpdateWorldPayload>
{
  public UpdateWorldValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());
  }
}
