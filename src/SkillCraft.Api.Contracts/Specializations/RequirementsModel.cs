using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Contracts.Specializations;

public record RequirementsModel
{
  public TalentModel? Talent { get; set; }
  public List<string> Other { get; set; } = [];
}
