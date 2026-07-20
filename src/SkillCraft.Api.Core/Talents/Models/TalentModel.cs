using Krakenar.Contracts;

namespace SkillCraft.Api.Core.Talents.Models;

public class TalentModel : Aggregate
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public Skill? Skill { get; set; }
  public TalentModel? RequiredTalent { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
