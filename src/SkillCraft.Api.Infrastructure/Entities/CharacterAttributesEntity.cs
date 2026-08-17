using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters;

namespace SkillCraft.Api.Infrastructure.Entities;

internal record CharacterAttributesEntity
{
  private const char Separator = '|';
  private const char PairSeparator = ':';
  private const char ValueSeparator = '+';

  public CharacterAttributeEntity Dexterity { get; set; } = new();
  public CharacterAttributeEntity Health { get; set; } = new();
  public CharacterAttributeEntity Intellect { get; set; } = new();
  public CharacterAttributeEntity Senses { get; set; } = new();
  public CharacterAttributeEntity Vigor { get; set; } = new();

  public CharacterAttributesEntity()
  {
  }

  public CharacterAttributesEntity(IStartingAttributes starting)
  {
    Dexterity = new CharacterAttributeEntity(starting.Dexterity);
    Health = new CharacterAttributeEntity(starting.Health);
    Intellect = new CharacterAttributeEntity(starting.Intellect);
    Senses = new CharacterAttributeEntity(starting.Senses);
    Vigor = new CharacterAttributeEntity(starting.Vigor);
  }

  public static CharacterAttributesEntity Parse(string? value)
  {
    CharacterAttributesEntity attributes = new();
    if (value is not null)
    {
      string[] values = value.Split(Separator);
      foreach (string encoded in values)
      {
        string[] parts = encoded.Split(PairSeparator);
        if (parts.Length == 2 && Enum.TryParse(parts[0], out GameAttribute attribute) && Enum.IsDefined(attribute))
        {
          string[] pair = parts[1].Split(ValueSeparator);
          if (pair.Length == 2 && int.TryParse(pair[0], out int starting) && int.TryParse(pair[1], out int progression))
          {
            switch (attribute)
            {
              case GameAttribute.Dexterity:
                attributes.Dexterity.Starting = starting;
                attributes.Dexterity.Progression = progression;
                break;
              case GameAttribute.Health:
                attributes.Health.Starting = starting;
                attributes.Health.Progression = progression;
                break;
              case GameAttribute.Intellect:
                attributes.Intellect.Starting = starting;
                attributes.Intellect.Progression = progression;
                break;
              case GameAttribute.Senses:
                attributes.Senses.Starting = starting;
                attributes.Senses.Progression = progression;
                break;
              case GameAttribute.Vigor:
                attributes.Vigor.Starting = starting;
                attributes.Vigor.Progression = progression;
                break;
            }
          }
        }
      }
    }
    return attributes;
  }

  public override string? ToString()
  {
    List<string> attributes = new(capacity: 5);
    if (Dexterity.Starting != 0 || Dexterity.Progression != 0)
    {
      attributes.Add(Encode(GameAttribute.Dexterity, Dexterity));
    }
    if (Health.Starting != 0 || Health.Progression != 0)
    {
      attributes.Add(Encode(GameAttribute.Health, Health));
    }
    if (Intellect.Starting != 0 || Intellect.Progression != 0)
    {
      attributes.Add(Encode(GameAttribute.Intellect, Intellect));
    }
    if (Senses.Starting != 0 || Senses.Progression != 0)
    {
      attributes.Add(Encode(GameAttribute.Senses, Senses));
    }
    if (Vigor.Starting != 0 || Vigor.Progression != 0)
    {
      attributes.Add(Encode(GameAttribute.Vigor, Vigor));
    }
    return attributes.Count < 1 ? null : string.Join(Separator, attributes);
  }
  private static string Encode(GameAttribute attribute, CharacterAttributeEntity entity)
  {
    return string.Join(PairSeparator, attribute, string.Join(ValueSeparator, entity.Starting, entity.Progression));
  }
}
