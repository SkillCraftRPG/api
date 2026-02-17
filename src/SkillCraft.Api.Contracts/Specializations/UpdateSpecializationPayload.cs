namespace SkillCraft.Api.Contracts.Specializations;

public record UpdateSpecializationPayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }
}
