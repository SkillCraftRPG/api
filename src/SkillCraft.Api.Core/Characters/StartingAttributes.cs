using FluentValidation;
using SkillCraft.Api.Contracts.Characters;
using SkillCraft.Api.Core.Characters.Validators;

namespace SkillCraft.Api.Core.Characters;

public record StartingAttributes : IStartingAttributes
{
  public const int MinimumValue = -2;
  public const int MaximumValue = 4;

  public int Dexterity { get; }
  public int Health { get; }
  public int Intellect { get; }
  public int Senses { get; }
  public int Vigor { get; }

  public StartingAttributes()
  {
  }

  public StartingAttributes(IStartingAttributes attributes)
    : this(attributes.Dexterity, attributes.Health, attributes.Intellect, attributes.Senses, attributes.Vigor)
  {
  }

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
}
