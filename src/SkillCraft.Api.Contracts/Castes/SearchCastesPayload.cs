using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Castes;

public record SearchCastesPayload : SearchPayload
{
  public new List<CasteSortOption> Sort { get; set; } = [];
}
