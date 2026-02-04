namespace SkillCraft.Api.Contracts.Worlds;

public record CreateOrReplaceWorldPayload
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
}
