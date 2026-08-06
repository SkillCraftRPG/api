namespace SkillCraft.Api.Core.Characters.Models;

public record AddCharacterTalentPayload : CharacterTalentPayload
{
  public Guid TalentId { get; set; }
}
