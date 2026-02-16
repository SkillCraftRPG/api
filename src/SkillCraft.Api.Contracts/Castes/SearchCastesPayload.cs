using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Castes;

public record SearchCastesPayload : SearchPayload
{
  public string? Skill { get; set; }

  public new List<CasteSortOption> Sort { get; set; } = [];
}
