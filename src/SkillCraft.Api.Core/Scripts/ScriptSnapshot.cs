using SkillCraft.Api.Core.Scripts.Events;

namespace SkillCraft.Api.Core.Scripts;

public record ScriptSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? Content { get; }

  public ScriptSnapshot(Script script)
  {
    Name = script.Name;
    Summary = script.Summary;
    Content = script.Content;
  }

  public ScriptUpdated? Compare(Script script)
  {
    int changes = 0;
    ScriptUpdated record = new(script);

    if (Name != script.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, script.Name);
    }

    if (Summary != script.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, script.Summary);
    }

    if (Content != script.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, script.Content);
    }

    return changes < 1 ? null : record;
  }
}
