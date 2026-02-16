using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Lineages;

public class LineageModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public LineageModel? Parent { get; set; }
  public List<LineageModel> Children { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
