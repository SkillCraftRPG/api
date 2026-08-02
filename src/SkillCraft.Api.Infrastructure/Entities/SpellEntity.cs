using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Core.Spells.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class SpellEntity : AggregateEntity
{
  public int SpellId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public int Tier { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Summary { get; private set; }
  public string? Content { get; private set; }

  public SpellEntity(Spell spell) : base(spell)
  {
    WorldId = spell.WorldId.ResourceId;
    Id = spell.ResourceId;

    Tier = spell.Tier.Value;

    Update(spell);
  }

  public SpellEntity(SpellCreated @event) : base(@event)
  {
    SpellId spellId = new(@event.StreamId);
    WorldId = spellId.WorldId.ResourceId;
    Id = spellId.ResourceId;

    Tier = @event.Tier.Value;
    Name = @event.Name.Value;
  }

  private SpellEntity() : base()
  {
  }

  public void Edit(SpellEdited @event)
  {
    base.Update(@event);

    Summary = @event.Summary?.Value;
    Content = @event.Content?.Value;
  }

  public void Rename(SpellRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void Update(Spell spell)
  {
    base.Update(spell);

    Name = spell.Name.Value;
    Summary = spell.Summary?.Value;
    Content = spell.Content?.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
