using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Lineages;

public record SearchLineagesPayload : SearchPayload
{
  public Guid? LanguageId { get; set; }
  public SizeCategory? SizeCategory { get; set; }

  public new List<LineageSortOption> Sort { get; set; } = [];
}
