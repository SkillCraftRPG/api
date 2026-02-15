namespace SkillCraft.Api.Contracts.Worlds;

public record UpdateWorldPayload
{
  public string? Name { get; set; }
  public Update<string>? Description { get; set; }
}
