namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterTalentDiscountModel : ICharacterTalentDiscount
{
  public CharacterTalentDiscountSource Source { get; set; }
  public string Target { get; set; } = string.Empty;
  public int Amount { get; set; }

  public CharacterTalentDiscountModel()
  {
  }

  [JsonConstructor]
  public CharacterTalentDiscountModel(CharacterTalentDiscountSource source, string target, int amount)
  {
    Source = source;
    Target = target;
    Amount = amount;
  }

  public CharacterTalentDiscountModel(ICharacterTalentDiscount discount) : this(discount.Source, discount.Target, discount.Amount)
  {
  }
}
