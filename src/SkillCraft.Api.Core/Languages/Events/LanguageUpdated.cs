namespace SkillCraft.Api.Core.Languages.Events;

public class LanguageUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Description { get; set; }

  public Change<Guid?>? ScriptId { get; set; }
  public Change<string>? TypicalSpeakers { get; set; }

  public LanguageUpdated() : base()
  {
  }

  public LanguageUpdated(Language language) : base(language)
  {
  }
}
