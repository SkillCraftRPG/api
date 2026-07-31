using SkillCraft.Api.Core.Languages.Events;

namespace SkillCraft.Api.Core.Languages;

public record LanguageSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? HtmlContent { get; }

  public Guid? ScriptId { get; }
  public string? TypicalSpeakers { get; }

  public LanguageSnapshot(Language language)
  {
    Name = language.Name;
    Summary = language.Summary;
    HtmlContent = language.HtmlContent;

    ScriptId = language.Script?.Id;
    TypicalSpeakers = language.TypicalSpeakers;
  }

  public LanguageUpdated? Compare(Language language)
  {
    int changes = 0;
    LanguageUpdated record = new(language);

    if (Name != language.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, language.Name);
    }

    if (Summary != language.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, language.Summary);
    }

    if (HtmlContent != language.HtmlContent)
    {
      changes++;
      record.HtmlContent = new Change<string>(HtmlContent, language.HtmlContent);
    }

    if (ScriptId != language.Script?.Id)
    {
      changes++;
      record.ScriptId = new Change<Guid?>(ScriptId, language.Script?.Id);
    }

    if (TypicalSpeakers != language.TypicalSpeakers)
    {
      changes++;
      record.TypicalSpeakers = new Change<string>(TypicalSpeakers, language.TypicalSpeakers);
    }

    return changes < 1 ? null : record;
  }
}
