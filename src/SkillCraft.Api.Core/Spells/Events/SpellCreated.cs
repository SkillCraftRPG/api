namespace SkillCraft.Api.Core.Spells.Events;

public class SpellCreated : CreateEvent
{
  public string Name { get; set; } = string.Empty;
  public string? Summary { get; set; }
  public string? Content { get; set; }

  public SpellCreated() : base()
  {
  }

  public SpellCreated(Spell spell) : base(spell)
  {
    Name = spell.Name;
    Summary = spell.Summary;
    Content = spell.Content;
  }
}
