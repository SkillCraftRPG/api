namespace SkillCraft.Api.Contracts.Lineages;

public record CreateOrReplaceLineagePayload
{
  public Guid? ParentId { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public SpeedsModel Speeds { get; set; } = new();
}
