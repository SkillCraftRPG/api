using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Lineages.Models;

public record SearchLineagesPayload : SearchPayload
{
  public SizeCategory? SizeCategory { get; set; }

  public new List<LineageSortOption> Sort { get; set; } = [];
}
