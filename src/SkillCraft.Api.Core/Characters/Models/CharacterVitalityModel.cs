namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterVitalityModel : ICharacterVitality
{
  public int Current { get; set; }
  public int Temporary { get; set; }
  public int Stun { get; set; }
}
