using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Educations.Models;

public record SearchEducationsPayload : SearchPayload
{
  public Skill? Skill { get; set; }

  public new List<EducationSortOption> Sort { get; set; } = [];
}
