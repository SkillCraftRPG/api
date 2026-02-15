using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Parties;

public record SearchPartiesPayload : SearchPayload
{
  public new List<PartySortOption> Sort { get; set; } = [];
}
