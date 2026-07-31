namespace SkillCraft.Api.Core.Spells.Events;

public class SpellUpdated : UpdateEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Summary { get; set; }
  public Change<string>? Content { get; set; }

  public SpellUpdated() : base()
  {
  }

  public SpellUpdated(Spell spell) : base(spell)
  {
  }
}
