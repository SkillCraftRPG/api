using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Contracts.Specializations;

public record OptionsModel
{
  public List<TalentModel> Talents { get; set; } = [];
  public List<string> Other { get; set; } = [];
}
