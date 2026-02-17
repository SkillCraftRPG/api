namespace SkillCraft.Api.Contracts.Specializations;

public record RequirementsPayload
{
  public Guid? TalentId { get; set; }
  public List<string> Other { get; set; } = [];
}
