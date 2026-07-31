using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Spells.Models;

public class SpellModel : Aggregate
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
