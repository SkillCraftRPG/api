using Logitar.EventSourcing;
using SkillCraft.Api.Core.Scripts.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Scripts;

public class Script : AggregateRoot, IResource
{
  public const string ResourceKind = "Script";

  public new ScriptId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId.ResourceId);

  public Script() : base()
  {
  }

  public Script(World world, Name name, ActorId? actorId = null)
    : this(ScriptId.NewId(world.Id), name, actorId)
  {
  }

  public Script(ScriptId scriptId, Name name, ActorId? actorId = null)
    : base(scriptId.StreamId)
  {
    Raise(new ScriptCreated(name), actorId);
  }
  protected virtual void Handle(ScriptCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new ScriptDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new ScriptEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(ScriptEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new ScriptRenamed(name), actorId);
    }
  }
  protected virtual void Handle(ScriptRenamed @event)
  {
    _name = @event.Name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
