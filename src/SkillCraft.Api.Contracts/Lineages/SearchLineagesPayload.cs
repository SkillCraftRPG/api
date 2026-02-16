using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Lineages;

public record SearchLineagesPayload : SearchPayload
{
  public new List<LineageSortOption> Sort { get; set; } = [];
}
