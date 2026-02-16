using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Educations;

public record SearchEducationsPayload : SearchPayload
{
  public string? Skill { get; set; }

  public new List<EducationSortOption> Sort { get; set; } = [];
}
