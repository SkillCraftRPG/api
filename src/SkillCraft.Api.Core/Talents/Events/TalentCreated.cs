namespace SkillCraft.Api.Core.Talents.Events;

public class TalentCreated : CreateEvent
{
  public int Tier { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? HtmlContent { get; set; }

  public bool AllowMultiplePurchases { get; set; }
  public Skill? Skill { get; set; }
  public Guid? RequiredTalentId { get; set; }

  public TalentCreated() : base()
  {
  }

  public TalentCreated(Talent talent) : base(talent)
  {
    Tier = talent.Tier;

    Name = talent.Name;
    Summary = talent.Summary;
    HtmlContent = talent.HtmlContent;

    AllowMultiplePurchases = talent.AllowMultiplePurchases;
    Skill = talent.Skill;
    RequiredTalentId = talent.RequiredTalent?.Id;
  }
}
