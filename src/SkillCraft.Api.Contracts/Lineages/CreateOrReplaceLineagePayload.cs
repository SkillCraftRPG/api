namespace SkillCraft.Api.Contracts.Lineages;

public record CreateOrReplaceLineagePayload
{
  public Guid? ParentId { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public List<FeatureModel> Features { get; set; } = [];
  public LanguagesPayload Languages { get; set; } = new();

  public SpeedsModel Speeds { get; set; } = new();
  public SizeModel Size { get; set; } = new();
  public WeightModel Weight { get; set; } = new();
  public AgeModel Age { get; set; } = new();
}
