namespace SkillCraft.Api.Core.Castes.Events;

public class CasteUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Summary { get; set; }
  public Change<string>? HtmlContent { get; set; }

  public Change<Skill?>? Skill { get; set; }
  public Change<string>? WealthRoll { get; set; }

  public CasteUpdated() : base()
  {
  }

  public CasteUpdated(Caste caste) : base(caste)
  {
  }
}
