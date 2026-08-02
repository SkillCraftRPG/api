using Logitar.EventSourcing;
using SkillCraft.Api.Core.Customizations.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations;

public class Customization : AggregateRoot, IResource
{
  public const string ResourceKind = "Customization";

  public new CustomizationId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  public CustomizationKind Kind { get; private set; }

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId.ResourceId);

  public Customization() : base()
  {
  }

  public Customization(World world, CustomizationKind kind, Name name, ActorId? actorId = null)
    : this(CustomizationId.NewId(world.Id), kind, name, actorId)
  {
  }

  public Customization(CustomizationId customizationId, CustomizationKind kind, Name name, ActorId? actorId = null)
    : base(customizationId.StreamId)
  {
    if (!Enum.IsDefined(kind))
    {
      throw new ArgumentOutOfRangeException(nameof(kind));
    }

    Raise(new CustomizationCreated(kind, name), actorId);
  }
  protected virtual void Handle(CustomizationCreated @event)
  {
    Kind = @event.Kind;

    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new CustomizationDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new CustomizationEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(CustomizationEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new CustomizationRenamed(name), actorId);
    }
  }
  protected virtual void Handle(CustomizationRenamed @event)
  {
    _name = @event.Name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
