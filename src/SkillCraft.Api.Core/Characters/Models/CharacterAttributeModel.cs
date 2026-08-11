namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterAttributeModel
{
  public int Starting { get; set; }
  public int Progression { get; set; }
  public int Bonus { get; set; }
  public int Total => Starting + Progression + Bonus;
}
