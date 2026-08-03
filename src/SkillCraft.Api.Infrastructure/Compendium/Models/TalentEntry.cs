using Krakenar.Contracts;

namespace SkillCraft.Api.Infrastructure.Compendium.Models;

internal class TalentEntry : Aggregate
{
  public string Slug { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;

  public int Tier { get; set; }
  public bool AllowMultiplePurchases { get; set; }
  public SkillEntry? Skill { get; set; }

  public string? MetaDescription { get; set; }
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public TalentEntry? RequiredTalent { get; set; }
  public List<TalentEntry> RequiringTalents { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
