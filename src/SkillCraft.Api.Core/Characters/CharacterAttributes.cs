namespace SkillCraft.Api.Core.Characters;

public record CharacterAttributes
{
  public CharacterAttribute Dexterity { get; }
  public CharacterAttribute Health { get; }
  public CharacterAttribute Intellect { get; }
  public CharacterAttribute Senses { get; }
  public CharacterAttribute Vigor { get; }

  public int TotalPoints { get; }
  public int SpentPoints { get; }
  public int RemainingPoints => TotalPoints - SpentPoints;

  public CharacterAttributes(IStartingAttributes starting, IReadOnlyDictionary<GameAttribute, int>? increases = null, int level = 0, IEnumerable<CharacterModifier>? modifiers = null)
  {
    Dictionary<GameAttribute, int> attributes = new(capacity: 5);
    if (modifiers is not null)
    {
      foreach (CharacterModifier modifier in modifiers)
      {
        if (modifier.Kind == CharacterModifierKind.Attribute)
        {
          GameAttribute attribute = Enum.Parse<GameAttribute>(modifier.Target);
          attributes[attribute] = attributes.GetValueOrDefault(attribute) + modifier.Value;
        }
      }
    }

    increases ??= new Dictionary<GameAttribute, int>();

    Dexterity = new CharacterAttribute(starting.Dexterity, increases.GetValueOrDefault(GameAttribute.Dexterity), attributes.GetValueOrDefault(GameAttribute.Dexterity));
    Health = new CharacterAttribute(starting.Health, increases.GetValueOrDefault(GameAttribute.Health), attributes.GetValueOrDefault(GameAttribute.Health));
    Intellect = new CharacterAttribute(starting.Intellect, increases.GetValueOrDefault(GameAttribute.Intellect), attributes.GetValueOrDefault(GameAttribute.Intellect));
    Senses = new CharacterAttribute(starting.Senses, increases.GetValueOrDefault(GameAttribute.Senses), attributes.GetValueOrDefault(GameAttribute.Senses));
    Vigor = new CharacterAttribute(starting.Vigor, increases.GetValueOrDefault(GameAttribute.Vigor), attributes.GetValueOrDefault(GameAttribute.Vigor));

    TotalPoints = (level + 5) / 10;
    SpentPoints = increases.Values.Sum();
  }
}
