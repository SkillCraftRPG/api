namespace SkillCraft.Api.Contracts.Talents;

public record CreateOrReplaceTalentPayload
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }
}
