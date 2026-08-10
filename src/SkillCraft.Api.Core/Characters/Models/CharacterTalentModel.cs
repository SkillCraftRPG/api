using Krakenar.Contracts.Actors;
using SkillCraft.Api.Core.Talents.Models;

namespace SkillCraft.Api.Core.Characters.Models;

public record CharacterTalentModel
{
  public Guid Id { get; set; }

  public TalentModel Talent { get; set; } = new();

  public string? Qualifier { get; set; }
  public string? Notes { get; set; }
  public List<CharacterTalentDiscountModel> Discounts { get; set; } = [];
  public int Cost => Math.Max(Talent.Cost - Discounts.Sum(discount => discount.Amount), 0);

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }
}
