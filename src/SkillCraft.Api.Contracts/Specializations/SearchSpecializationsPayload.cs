using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Specializations;

public record SearchSpecializationsPayload : SearchPayload
{
  public List<int> Tiers { get; set; } = [];
  public string? RequiredTalent { get; set; }

  public new List<SpecializationSortOption> Sort { get; set; } = [];
}
