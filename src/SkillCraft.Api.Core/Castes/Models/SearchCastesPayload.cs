using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Castes.Models;

public record SearchCastesPayload : SearchPayload
{
  public Skill? Skill { get; set; }

  public new List<CasteSortOption> Sort { get; set; } = [];
}
