using Logitar.EventSourcing;
using SkillCraft.Api.Core.Specializations.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Specializations;

public class Specialization : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Specialization";

  private SpecializationUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Summary is not null || _updated.Description is not null; // TODO(fpion): other properties

  public new SpecializationId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Tier? _tier = null;
  public Tier Tier => _tier ?? throw new InvalidOperationException("The specialization has not been initialized.");

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException("The specialization has not been initialized.");
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

  // TODO(fpion): Requirements { Talent, Other }
  // TODO(fpion): Options { Talents, Other }
  // TODO(fpion): Doctrine { Name, Description, DiscountedTalents, Features }

  public Specialization() : base()
  {
  }

  public Specialization(World world, Tier tier, Name name, UserId? userId = null, SpecializationId? specializationId = null)
    : this(world.Id, tier, name, userId ?? world.OwnerId, specializationId)
  {
  }
  public Specialization(WorldId worldId, Tier tier, Name name, UserId userId, SpecializationId? specializationId = null)
    : base((specializationId ?? SpecializationId.NewId(worldId)).StreamId)
  {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tier.Value, nameof(tier));

    Raise(new SpecializationCreated(tier, name), userId.ActorId);
  }
  protected virtual void Handle(SpecializationCreated @event)
  {
    _tier = @event.Tier;

    _name = @event.Name;
  }

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0); // TODO(fpion): other properties

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new SpecializationDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new SpecializationUpdated();
    }
  }
  protected virtual void Handle(SpecializationUpdated @event)
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

    // TODO(fpion): other properties
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
