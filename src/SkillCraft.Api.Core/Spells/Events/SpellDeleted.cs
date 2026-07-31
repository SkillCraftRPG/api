namespace SkillCraft.Api.Core.Spells.Events;

public class SpellDeleted : DeleteEvent
{
  public SpellDeleted() : base()
  {
  }

  public SpellDeleted(Spell spell, Guid userId) : base(spell, userId)
  {
  }
}
