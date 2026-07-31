using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Items.Models;

public class ItemModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public double? Price { get; set; }
  public double? Weight { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
