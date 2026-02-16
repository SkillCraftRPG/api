namespace SkillCraft.Api.Contracts.Lineages;

public record UpdateLineagePayload
{
  public string? Name { get; set; }
  public Update<string>? Summary { get; set; }
  public Update<string>? Description { get; set; }

  public List<FeatureModel>? Features { get; set; }
  public LanguagesPayload? Languages { get; set; }

  public SpeedsModel? Speeds { get; set; }
  public SizeModel? Size { get; set; }
  public WeightModel? Weight { get; set; }
  public AgeModel? Age { get; set; }
}
