namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterPointsModel
{
  public int Attributes { get; set; }
  public int Skills { get; set; }
  public int Talents { get; set; }
}
