namespace SkillCraft.Api.Core.Characters;

public record CharacterAttribute
{
  public int Starting { get; }
  public int Progression { get; }
  public int Modifiers { get; }
  public int Total => Starting + Progression + Modifiers;

  public CharacterAttribute(int starting, int progression = 0, int modifiers = 0)
  {
    Starting = starting;
    Progression = progression;
    Modifiers = modifiers;
  }
}
