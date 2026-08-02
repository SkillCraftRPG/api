namespace SkillCraft.Api.Core.Languages.Events;

public class LanguageCreated : CreateEvent
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public Guid? ScriptId { get; set; }
  public string? TypicalSpeakers { get; set; }

  public LanguageCreated() : base()
  {
  }

  public LanguageCreated(Language language) : base(language)
  {
    Name = language.Name;
    Summary = language.Summary;
    Content = language.Content;

    ScriptId = language.ScriptUid;
    TypicalSpeakers = language.TypicalSpeakers;
  }
}
