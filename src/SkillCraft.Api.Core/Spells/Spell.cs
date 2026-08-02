using Logitar.EventSourcing;
using SkillCraft.Api.Core.Spells.Events;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Spells;

public class Spell : AggregateRoot, IResource
{
  public const string ResourceKind = "Spell";

  public new SpellId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  private TalentTier? _tier = null;
  public TalentTier Tier => _tier ?? throw new InvalidOperationException("The tier has not been initialized.");

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Spell() : base()
  {
  }

  public Spell(World world, TalentTier tier, Name name, ActorId? actorId = null)
    : this(SpellId.NewId(world.Id), tier, name, actorId)
  {
  }

  public Spell(SpellId spellId, TalentTier tier, Name name, ActorId? actorId = null)
    : base(spellId.StreamId)
  {
    Raise(new SpellCreated(tier, name), actorId);
  }
  protected virtual void Handle(SpellCreated @event)
  {
    _tier = @event.Tier;
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new SpellDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new SpellEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(SpellEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new SpellRenamed(name), actorId);
    }
  }
  protected virtual void Handle(SpellRenamed @event)
  {
    _name = @event.Name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
