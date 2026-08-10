namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterAttributeModel
{
  public int Starting { get; set; }
  public int Spent { get; set; }
  public int Bonus { get; set; }
  public int Total { get; set; }
}
