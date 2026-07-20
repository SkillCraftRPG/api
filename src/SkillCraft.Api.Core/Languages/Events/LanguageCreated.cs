namespace SkillCraft.Api.Core.Languages.Events;

public class LanguageCreated : CreateEvent
{
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }

  public Guid? ScriptId { get; set; }
  public string? TypicalSpeakers { get; set; }

  public LanguageCreated() : base()
  {
  }

  public LanguageCreated(Language language) : base(language)
  {
    Name = language.Name;
    Description = language.Description;

    ScriptId = language.Script?.Id;
    TypicalSpeakers = language.TypicalSpeakers;
  }
}
