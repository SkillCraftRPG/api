using SkillCraft.Api.Core.Spells.Events;

namespace SkillCraft.Api.Core.Spells;

public record SpellSnapshot
{
  public string Name { get; }
  public string? Summary { get; }
  public string? Content { get; }

  public SpellSnapshot(Spell spell)
  {
    Name = spell.Name;
    Summary = spell.Summary;
    Content = spell.Content;
  }

  public SpellUpdated? Compare(Spell spell)
  {
    int changes = 0;
    SpellUpdated record = new(spell);

    if (Name != spell.Name)
    {
      changes++;
      record.Name = new Change<string>(Name, spell.Name);
    }

    if (Summary != spell.Summary)
    {
      changes++;
      record.Summary = new Change<string>(Summary, spell.Summary);
    }

    if (Content != spell.Content)
    {
      changes++;
      record.Content = new Change<string>(Content, spell.Content);
    }

    return changes < 1 ? null : record;
  }
}
