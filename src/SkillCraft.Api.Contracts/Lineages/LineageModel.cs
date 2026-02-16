using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Lineages;

public class LineageModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public List<FeatureModel> Features { get; set; } = [];
  public LanguagesModel Languages { get; set; } = new();

  public SpeedsModel Speeds { get; set; } = new();
  public SizeModel Size { get; set; } = new();
  public WeightModel Weight { get; set; } = new();
  public AgeModel Age { get; set; } = new();

  public LineageModel? Parent { get; set; }
  public List<LineageModel> Children { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
