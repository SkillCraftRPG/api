using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Talents;

public record SearchTalentsPayload : SearchPayload
{
  public List<int> Tiers { get; set; } = [];
  public bool? AllowMultiplePurchases { get; set; }
  public string? Skill { get; set; }
  public string? RequiredTalent { get; set; }

  public new List<TalentSortOption> Sort { get; set; } = [];
}
