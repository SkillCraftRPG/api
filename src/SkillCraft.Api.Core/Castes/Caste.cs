using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Castes;

public class Caste : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Caste";

  private CasteUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Summary is not null || _updated.Description is not null;

  public new CasteId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException("The caste has not been initialized.");
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

  public Caste() : base()
  {
  }

  public Caste(World world, Name name, UserId? userId = null, CasteId? casteId = null)
    : this(world.Id, name, userId ?? world.OwnerId, casteId)
  {
  }
  public Caste(WorldId worldId, Name name, UserId userId, CasteId? casteId = null)
    : base((casteId ?? CasteId.NewId(worldId)).StreamId)
  {
    Raise(new CasteCreated(name), userId.ActorId);
  }
  protected virtual void Handle(CasteCreated @event)
  {
    _name = @event.Name;
  }

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0);

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new CasteDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new CasteUpdated();
    }
  }
  protected virtual void Handle(CasteUpdated @event)
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
