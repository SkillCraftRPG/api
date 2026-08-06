namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterTalentDiscountModel : ICharacterTalentDiscount
{
  public CharacterTalentDiscountSource Source { get; set; }
  public string Target { get; set; } = string.Empty;
  public int Amount { get; set; }
}
