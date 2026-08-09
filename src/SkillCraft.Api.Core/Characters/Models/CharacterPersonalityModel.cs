namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterPersonalityModel : ICharacterPersonality
{
  public string? Traits { get; set; }
  public string? Ideals { get; set; }
  public string? Flaws { get; set; }
}
