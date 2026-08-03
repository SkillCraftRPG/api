using Krakenar.Contracts;

namespace SkillCraft.Api.Infrastructure.Compendium.Models;

internal class EducationEntry : Aggregate
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public int? WealthMultiplier { get; set; }
  public SkillEntry? Skill { get; set; }
  public FeatureEntry? Feature { get; set; }

  public string? MetaDescription { get; set; }
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
