using Krakenar.Contracts;

namespace SkillCraft.Api.Contracts.Talents;

public class TalentModel : Aggregate
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Description { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public GameSkill? Skill { get; set; }
  public TalentModel? RequiredTalent { get; set; }
  public List<TalentModel> RequiringTalents { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}
