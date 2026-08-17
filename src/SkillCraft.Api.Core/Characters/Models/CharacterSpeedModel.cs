namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterSpeedModel
{
  public int Lineage { get; set; }
  public int Modifiers { get; set; }
  public int Encumbrance { get; set; }
  public int Total => Lineage + Modifiers + Encumbrance;
}
