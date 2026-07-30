using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Lineages.Models;

public class LineageModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public List<LineageFeatureModel> Features { get; set; } = [];
  public LineageLanguagesModel Languages { get; set; } = new();
  public LineageNamesModel Names { get; set; } = new();
  public LineageSpeedsModel Speeds { get; set; } = new();
  public LineageSizeModel Size { get; set; } = new();
  public LineageWeightModel Weight { get; set; } = new();
  public LineageAgeModel Age { get; set; } = new();

  public LineageModel? Parent { get; set; }
  public List<LineageModel> Children { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
