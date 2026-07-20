using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Talents.Models;

public record SearchTalentsPayload : SearchPayload
{
  public List<int> Tiers { get; set; } = [];
  public bool? AllowMultiplePurchases { get; set; }
  public Skill? Skill { get; set; }
  public Guid? RequiredTalentId { get; set; }

  public new List<TalentSortOption> Sort { get; set; } = [];
}
