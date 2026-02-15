namespace SkillCraft.Api.Contracts.Customizations;

public record UpdateCustomizationPayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }
}
