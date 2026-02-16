namespace SkillCraft.Api.Contracts.Scripts;

public record CreateOrReplaceScriptPayload
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }
}
