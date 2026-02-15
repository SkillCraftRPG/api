using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Educations;

public record SearchEducationsPayload : SearchPayload
{
  public GameSkill? Skill { get; set; }

  public new List<EducationSortOption> Sort { get; set; } = [];
}
