namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterStatisticModel
{
  public int Base { get; set; }
  public int Bonus { get; set; }
  public int Total { get; set; }
}
