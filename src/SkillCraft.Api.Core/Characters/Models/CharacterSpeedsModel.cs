namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterSpeedsModel
{
  public CharacterSpeedModel Walk { get; set; } = new();
  public CharacterSpeedModel Climb { get; set; } = new();
  public CharacterSpeedModel Swim { get; set; } = new();
  public CharacterSpeedModel Fly { get; set; } = new();
  public bool Hover { get; set; }
  public CharacterSpeedModel Burrow { get; set; } = new();
}
