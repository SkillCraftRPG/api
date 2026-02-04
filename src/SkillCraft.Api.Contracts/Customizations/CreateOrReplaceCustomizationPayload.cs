namespace SkillCraft.Api.Contracts.Customizations;

public record CreateOrReplaceCustomizationPayload
{
  public CustomizationKind Kind { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }
}
