using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Castes.Models;

public class CasteModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public Skill? Skill { get; set; }
  public string? WealthRoll { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
