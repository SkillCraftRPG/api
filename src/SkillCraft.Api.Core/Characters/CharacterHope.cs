using FluentValidation;

namespace SkillCraft.Api.Core.Characters;

public interface ICharacterHope
{
  int Current { get; }
  int Maximum { get; }
}

public record CharacterHope : ICharacterHope
{
  public int Current { get; }
  public int Maximum { get; }

  public CharacterHope()
  {
  }

  [JsonConstructor]
  public CharacterHope(int current, int maximum)
  {
    Current = current;
    Maximum = maximum;
    new CharacterHopeValidator().ValidateAndThrow(this);
  }

  public CharacterHope(ICharacterHope hope) : this(hope.Current, hope.Maximum)
  {
  }
}

internal class CharacterHopeValidator : AbstractValidator<ICharacterHope>
{
  public CharacterHopeValidator()
  {
    RuleFor(x => x.Current).InclusiveBetween(0, 3).LessThanOrEqualTo(hope => hope.Maximum);
    RuleFor(x => x.Maximum).InclusiveBetween(0, 3);
  }
}
