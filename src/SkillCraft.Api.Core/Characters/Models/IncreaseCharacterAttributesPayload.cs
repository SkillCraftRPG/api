using FluentValidation;

namespace SkillCraft.Api.Core.Characters.Models;

public record IncreaseCharacterAttributesPayload
{
  public int Dexterity { get; set; }
  public int Health { get; set; }
  public int Intellect { get; set; }
  public int Senses { get; set; }
  public int Vigor { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<IncreaseCharacterAttributesPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Dexterity).InclusiveBetween(0, 9);
      RuleFor(x => x.Health).InclusiveBetween(0, 9);
      RuleFor(x => x.Intellect).InclusiveBetween(0, 9);
      RuleFor(x => x.Senses).InclusiveBetween(0, 9);
      RuleFor(x => x.Vigor).InclusiveBetween(0, 9);
      RuleFor(x => x).Must(payload => payload.Dexterity + payload.Health + payload.Intellect + payload.Senses + payload.Vigor > 0)
        .WithErrorCode("SpentPointsValidator")
        .WithMessage("At least one attribute point must be spent.");
    }
  }
}
