using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Castes;

public record SearchCastesPayload : SearchPayload
{
  public GameSkill? Skill { get; set; }

  public new List<CasteSortOption> Sort { get; set; } = [];
}
