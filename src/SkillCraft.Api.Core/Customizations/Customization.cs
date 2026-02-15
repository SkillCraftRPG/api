using Logitar.EventSourcing;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Core.Customizations.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Customizations;

public class Customization : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Customization";

  private CustomizationUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Summary is not null || _updated.Description is not null;

  public new CustomizationId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  public CustomizationKind Kind { get; private set; }

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException("The customization has not been initialized.");
    set
    {
      if (_name != value)
      {
        _name = value;
        _updated.Name = value;
      }
    }
  }
  private Summary? _summary = null;
  public Summary? Summary
  {
    get => _summary;
    set
    {
      if (_summary != value)
      {
        _summary = value;
        _updated.Summary = new Change<Summary>(value);
      }
    }
  }
  private Description? _description = null;
  public Description? Description
  {
    get => _description;
    set
    {
      if (_description != value)
      {
        _description = value;
        _updated.Description = new Change<Description>(value);
      }
    }
  }

  public Customization() : base()
  {
  }

  public Customization(World world, CustomizationKind kind, Name name, UserId? userId = null, CustomizationId? customizationId = null)
    : this(world.Id, kind, name, userId ?? world.OwnerId, customizationId)
  {
  }
  public Customization(WorldId worldId, CustomizationKind kind, Name name, UserId userId, CustomizationId? customizationId = null)
    : base((customizationId ?? CustomizationId.NewId(worldId)).StreamId)
  {
    if (!Enum.IsDefined(kind))
    {
      throw new ArgumentOutOfRangeException(nameof(kind));
    }

    Raise(new CustomizationCreated(kind, name), userId.ActorId);
  }
  protected virtual void Handle(CustomizationCreated @event)
  {
    Kind = @event.Kind;

    _name = @event.Name;
  }

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0);

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new CustomizationDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new CustomizationUpdated();
    }
  }
  protected virtual void Handle(CustomizationUpdated @event)
  {
    if (@event.Name is not null)
    {
      _name = @event.Name;
    }
    if (@event.Summary is not null)
    {
      _summary = @event.Summary.Value;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
