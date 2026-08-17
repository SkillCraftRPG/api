namespace SkillCraft.Api.Core.Characters;

public record CharacterModifier
{
  public CharacterModifierKind Kind { get; }
  public string Target { get; }

  public int Value { get; }

  public Name? Name { get; }
  public Notes? Notes { get; }

  public CharacterModifier(CharacterModifierKind kind, string target, int value, Name? name = null, Notes? notes = null)
  {
    if (value == 0)
    {
      throw new ArgumentOutOfRangeException(nameof(value), "The value must not equal 0.");
    }

    Kind = kind;
    Target = ParseTarget(kind, target) ?? throw new ArgumentOutOfRangeException(nameof(target));

    Value = value;

    Name = name;
    Notes = notes;
  }

  private static string? ParseTarget(CharacterModifierKind kind, string target) => kind switch
  {
    CharacterModifierKind.Attribute => Enum.TryParse(target, ignoreCase: true, out GameAttribute attribute) && Enum.IsDefined(attribute) ? attribute.ToString() : null,
    CharacterModifierKind.Skill => Enum.TryParse(target, ignoreCase: true, out Skill skill) && Enum.IsDefined(skill) ? skill.ToString() : null,
    CharacterModifierKind.Speed => Enum.TryParse(target, ignoreCase: true, out Speed speed) && Enum.IsDefined(speed) ? speed.ToString() : null,
    CharacterModifierKind.Statistic => Enum.TryParse(target, ignoreCase: true, out Statistic statistic) && Enum.IsDefined(statistic) ? statistic.ToString() : null,
    _ => throw new ArgumentOutOfRangeException(nameof(kind)),
  };
}
