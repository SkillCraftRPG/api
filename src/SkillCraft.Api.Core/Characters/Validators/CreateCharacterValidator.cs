using FluentValidation;
using SkillCraft.Api.Contracts.Characters;

namespace SkillCraft.Api.Core.Characters.Validators;

internal class CreateCharacterValidator : AbstractValidator<CreateCharacterPayload>
{
  public CreateCharacterValidator()
  {
    RuleFor(x => x.Name).Name();

    RuleFor(x => x.Characteristics).SetValidator(new CharacteristicsValidator());
    RuleFor(x => x.StartingAttributes).SetValidator(new StartingAttributesValidator());
  }
}
