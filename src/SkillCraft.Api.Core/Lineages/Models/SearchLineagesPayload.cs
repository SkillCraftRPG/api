using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Lineages.Models;

public record SearchLineagesPayload : SearchPayload
{
  public Guid? ParentId { get; set; }
  public SizeCategory? SizeCategory { get; set; }

  public new List<LineageSortOption> Sort { get; set; } = [];
}
