using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Talents;

public record UpdateTalentPayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }
}
