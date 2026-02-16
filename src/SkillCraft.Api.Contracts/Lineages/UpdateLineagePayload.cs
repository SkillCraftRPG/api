namespace SkillCraft.Api.Contracts.Lineages;

public record UpdateLineagePayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }

  public SpeedsModel? Speeds { get; set; }
  public SizeModel? Size { get; set; }
}
