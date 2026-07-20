using Krakenar.Contracts;
using SkillCraft.Api.Core.Features;

namespace SkillCraft.Api.Core.Educations.Models;

public class EducationModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public Skill? Skill { get; set; }
  public int? WealthMultiplier { get; set; }
  public FeatureModel? Feature { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
