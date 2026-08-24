using FluentValidation;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterVitality
{
  int Current { get; }
  int Temporary { get; }
  int Stun { get; }
}

public record CharacterVitality : ICharacterVitality
{
  public int Current { get; }
  public int Temporary { get; }
  public int Stun { get; }

  public CharacterVitality() : this(current: 0)
  {
  }

  [JsonConstructor]
  public CharacterVitality(int current, int temporary = 0, int stun = 0)
  {
    Current = current;
    Temporary = temporary;
    Stun = stun;
    new CharacterVitalityValidator().ValidateAndThrow(this);
  }

  public CharacterVitality(ICharacterVitality vitality) : this(vitality.Current, vitality.Temporary, vitality.Stun)
  {
  }
}

internal class CharacterVitalityValidator : AbstractValidator<ICharacterVitality>
{
  public CharacterVitalityValidator()
  {
    RuleFor(x => x.Current).InclusiveBetween(0, 999);
    RuleFor(x => x.Temporary).InclusiveBetween(0, 999);
    RuleFor(x => x.Stun).InclusiveBetween(0, 999);
  }
}
