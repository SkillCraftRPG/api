using Logitar.EventSourcing;
using SkillCraft.Api.Core.Parties.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Parties;

public class Party : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Party";

  private PartyUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Description is not null;

  public new PartyId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException("The party has not been initialized.");
    set
    {
      if (_name != value)
      {
        _name = value;
        _updated.Name = value;
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

  public Party() : base()
  {
  }

  public Party(World world, Name name, UserId? userId = null, PartyId? partyId = null)
    : this(world.Id, name, userId ?? world.OwnerId, partyId)
  {
  }
  public Party(WorldId worldId, Name name, UserId userId, PartyId? partyId = null)
    : base((partyId ?? PartyId.NewId(worldId)).StreamId)
  {
    Raise(new PartyCreated(name), userId.ActorId);
  }
  protected virtual void Handle(PartyCreated @event)
  {
    _name = @event.Name;
  }

  public long CalculateSize() => Name.Size + (Description?.Size ?? 0);

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new PartyDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new PartyUpdated();
    }
  }
  protected virtual void Handle(PartyUpdated @event)
  {
    if (@event.Name is not null)
    {
      _name = @event.Name;
    }
    if (@event.Description is not null)
    {
      _description = @event.Description.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
