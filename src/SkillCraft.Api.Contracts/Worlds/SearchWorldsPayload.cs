using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Worlds;

public record SearchWorldsPayload : SearchPayload
{
  public new List<WorldSortOption> Sort { get; set; } = [];
}
