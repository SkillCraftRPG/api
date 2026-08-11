namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterSkillModel
{
  public int Rank { get; set; }
  public int Talents { get; set; }
  public int Attribute { get; set; }
  public int Bonus { get; set; }
  public int Total => (Talents < 1 ? (Rank / 2) : Rank) + Talents + Attribute + Bonus;
}
