using SkillCraft.Api.Core.Talents.Models;

namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterTalentModel
{
  public TalentModel Talent { get; set; } = new();

  public string? Qualifier { get; set; }
  public string? Notes { get; set; }
  public List<CharacterTalentDiscountModel> Discounts { get; set; } = [];
}
