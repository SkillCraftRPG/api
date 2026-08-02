using Logitar.EventSourcing;
using SkillCraft.Api.Core.Identity;
using SkillCraft.Api.Core.Worlds.Events;

namespace SkillCraft.Api.Core.Worlds;

public class World : AggregateRoot, IResource
{
  public const string ResourceKind = "World";

  public new WorldId Id => new(base.Id);
  public Guid ResourceId => Id.ResourceId;

  public UserId OwnerId { get; private set; }

  private Key? _key = null;
  public Key Key => _key ?? throw new InvalidOperationException("The key has not been initialized.");
  public Name? Name { get; private set; }
  public Content? Content { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId);

  public World() : base()
  {
  }

  public World(UserId ownerId, Key key, WorldId? worldId = null)
    : base((worldId ?? WorldId.NewId()).StreamId)
  {
    Raise(new WorldCreated(ownerId, key), ownerId.ActorId);
  }
  protected virtual void Handle(WorldCreated @event)
  {
    OwnerId = @event.OwnerId;

    _key = @event.Key;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new WorldDeleted(), actorId);
    }
  }

  public void Edit(Content? content, ActorId? actorId = null)
  {
    if (!Equals(Content, content))
    {
      Raise(new WorldEdited(content), actorId);
    }
  }
  protected virtual void Handle(WorldEdited @event)
  {
    Content = @event.Content;
  }

  public void Rename(Name? name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new WorldRenamed(name), actorId);
    }
  }
  protected virtual void Handle(WorldRenamed @event)
  {
    Name = @event.Name;
  }

  public void SetKey(Key key, ActorId? actorId = null)
  {
    if (!Equals(Key, key))
    {
      Raise(new WorldKeyChanged(key), actorId);
    }
  }
  protected virtual void Handle(WorldKeyChanged @event)
  {
    _key = @event.Key;
  }

  public override string ToString() => $"{Name?.Value ?? Key.Value} | {base.ToString()}";
}
