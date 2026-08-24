namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterHopeModel : ICharacterHope
{
  public int Current { get; set; }
  public int Maximum { get; set; }
}
