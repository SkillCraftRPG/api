using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Talents;

public record SearchTalentsPayload : SearchPayload
{
  // TODO(fpion): Tier
  public bool? AllowMultiplePurchases { get; set; }
  // TODO(fpion): Skill
  // TODO(fpion): RequiredTalent

  public new List<TalentSortOption> Sort { get; set; } = [];
}
