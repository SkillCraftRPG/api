using FluentValidation;

namespace SkillCraft.Api.Core.Characters.Models;

public record GainCharacterExperiencePayload
{
  public int Experience { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<GainCharacterExperiencePayload>
  {
    public Validator()
    {
      RuleFor(x => x.Experience).GreaterThan(0);
    }
  }
}
