namespace SkillCraft.Api.Contracts.Specializations;

public record OptionsPayload
{
  public List<Guid> TalentIds { get; set; } = [];
  public List<string> Other { get; set; } = [];
}
