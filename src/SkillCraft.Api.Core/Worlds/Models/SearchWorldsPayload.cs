using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Worlds.Models;

public record SearchWorldsPayload : SearchPayload
{
  public new List<WorldSortOption> Sort { get; set; } = [];
}
