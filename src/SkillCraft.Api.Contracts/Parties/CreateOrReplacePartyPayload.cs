namespace SkillCraft.Api.Contracts.Parties;

public record CreateOrReplacePartyPayload
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
}
