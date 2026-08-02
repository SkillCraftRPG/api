using Logitar.EventSourcing;
using SkillCraft.Api.Core.Items.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Items;

public class Item : AggregateRoot, IResource
{
  public const string ResourceKind = "Item";

  public new ItemId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  public Price? Price { get; private set; }
  public Weight? Weight { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId.ResourceId);

  public Item() : base()
  {
  }

  public Item(World world, Name name, ActorId? actorId = null)
    : this(ItemId.NewId(world.Id), name, actorId)
  {
  }

  public Item(ItemId itemId, Name name, ActorId? actorId = null)
    : base(itemId.StreamId)
  {
    Raise(new ItemCreated(name), actorId);
  }
  protected virtual void Handle(ItemCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ItemDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new ItemEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(ItemEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new ItemRenamed(name), actorId);
    }
  }
  protected virtual void Handle(ItemRenamed @event)
  {
    _name = @event.Name;
  }

  public void SetRules(Price? price, Weight? weight, ActorId? actorId = null)
  {
    if (!Equals(Price, price) || !Equals(Weight, weight))
    {
      Raise(new ItemRulesChanged(price, weight), actorId);
    }
  }
  protected virtual void Handle(ItemRulesChanged @event)
  {
    Price = @event.Price;
    Weight = @event.Weight;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
