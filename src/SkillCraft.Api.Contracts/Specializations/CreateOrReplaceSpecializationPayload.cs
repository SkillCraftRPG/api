namespace SkillCraft.Api.Contracts.Specializations;

public record CreateOrReplaceSpecializationPayload
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public RequirementsPayload Requirements { get; set; } = new();
  // TODO(fpion): Options { Talents, Other }
  // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }
}
