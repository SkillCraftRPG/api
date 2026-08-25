namespace SkillCraft.Api.Core.Characters;

public record CharacterStatistic
{
  public int Base { get; }
  public int Modifiers { get; }
  public int Total => Base + Modifiers;

  public CharacterStatistic(int @base, int modifiers)
  {
    Base = @base;
    Modifiers = modifiers;
  }
}
