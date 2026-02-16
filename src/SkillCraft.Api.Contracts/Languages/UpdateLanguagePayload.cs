using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Languages;

public record UpdateLanguagePayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }
}
