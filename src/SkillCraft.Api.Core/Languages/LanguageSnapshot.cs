using SkillCraft.Api.Core.Languages.Events;

namespace SkillCraft.Api.Core.Languages;

public record LanguageSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? Content { get; }

  public Guid? ScriptId { get; }
  public string? TypicalSpeakers { get; }

  public LanguageSnapshot(Language language)
  {
    Name = language.Name;
    Summary = language.Summary;
    Content = language.Content;

    ScriptId = language.ScriptUid;
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

    if (Content != language.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, language.Content);
    }

    if (ScriptId != language.ScriptUid)
    {
      changes++;
      record.ScriptId = new Change<Guid?>(ScriptId, language.ScriptUid);
    }

    if (TypicalSpeakers != language.TypicalSpeakers)
    {
      changes++;
      record.TypicalSpeakers = new Change<string>(TypicalSpeakers, language.TypicalSpeakers);
    }

    return changes < 1 ? null : record;
  }
}
