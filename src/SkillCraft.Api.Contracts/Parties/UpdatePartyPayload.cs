namespace SkillCraft.Api.Contracts.Parties;

public record UpdatePartyPayload
{
  public string? Name { get; set; }
  public Update<string>? Description { get; set; }
}
