namespace SkillCraft.Api.Contracts.Languages;

public record CreateOrReplaceLanguagePayload
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }
}
