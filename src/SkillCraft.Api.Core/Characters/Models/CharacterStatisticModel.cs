namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterStatisticModel
{
  public int Base { get; set; }
  public int Modifiers { get; set; }
  public int Total => Base + Modifiers;
}
