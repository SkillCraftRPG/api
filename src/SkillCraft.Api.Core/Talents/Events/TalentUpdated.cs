namespace SkillCraft.Api.Core.Talents.Events;

public class TalentUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Summary { get; set; }
  public Change<string>? HtmlContent { get; set; }

  public Change<bool>? AllowMultiplePurchases { get; set; }
  public Change<Skill?>? Skill { get; set; }
  public Change<Guid?>? RequiredTalentId { get; set; }

  public TalentUpdated() : base()
  {
  }

  public TalentUpdated(Talent talent) : base(talent)
  {
  }
}
