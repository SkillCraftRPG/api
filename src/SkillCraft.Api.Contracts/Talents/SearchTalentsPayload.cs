using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Talents;

public record SearchTalentsPayload : SearchPayload
{
  public new List<TalentSortOption> Sort { get; set; } = [];
}
