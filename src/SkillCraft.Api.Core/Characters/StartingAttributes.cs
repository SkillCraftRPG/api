using FluentValidation;

namespace SkillCraft.Api.Core.Characters;

public interface IStartingAttributes
{
  int Dexterity { get; }
  int Health { get; }
  int Intellect { get; }
  int Senses { get; }
  int Vigor { get; }
}

public record StartingAttributes : IStartingAttributes
{
  public int Dexterity { get; }
  public int Health { get; }
  public int Intellect { get; }
  public int Senses { get; }
  public int Vigor { get; }

  [JsonConstructor]
  public StartingAttributes(int dexterity, int health, int intellect, int senses, int vigor)
  {
    Dexterity = dexterity;
    Health = health;
    Intellect = intellect;
    Senses = senses;
    Vigor = vigor;
    new StartingAttributesValidator().ValidateAndThrow(this);
  }

  public StartingAttributes(IStartingAttributes attributes)
    : this(attributes.Dexterity, attributes.Health, attributes.Intellect, attributes.Senses, attributes.Vigor)
  {
  }
}

internal class StartingAttributesValidator : AbstractValidator<IStartingAttributes>
{
  private const int MinimumValue = -2;
  private const int MaximumValue = 4;

  public StartingAttributesValidator()
  {
    RuleFor(x => x.Dexterity).InclusiveBetween(MinimumValue, MaximumValue);
    RuleFor(x => x.Health).InclusiveBetween(MinimumValue, MaximumValue);
    RuleFor(x => x.Intellect).InclusiveBetween(MinimumValue, MaximumValue);
    RuleFor(x => x.Senses).InclusiveBetween(MinimumValue, MaximumValue);
    RuleFor(x => x.Vigor).InclusiveBetween(MinimumValue, MaximumValue);

    RuleFor(x => x).Must(x => (x.Dexterity + x.Health + x.Intellect + x.Senses + x.Vigor) == 0)
      .WithErrorCode("StartingAttributesValidator")
      .WithMessage("The starting attributes sum must equal 0.");
  }
}
