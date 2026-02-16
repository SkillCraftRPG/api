using Logitar.EventSourcing;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Talents;

public class Talent : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Talent";

  private TalentUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Summary is not null || _updated.Description is not null
    || _updated.AllowMultiplePurchases is not null || _updated.Skill is not null || _updated.RequiredTalentId is not null;

  public new TalentId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Tier? _tier = null;
  public Tier Tier => _tier ?? throw new InvalidOperationException("The talent has not been initialized.");

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException("The talent has not been initialized.");
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

  private bool _allowMultiplePurchases = false;
  public bool AllowMultiplePurchases
  {
    get => _allowMultiplePurchases;
    set
    {
      if (_allowMultiplePurchases != value)
      {
        _allowMultiplePurchases = value;
        _updated.AllowMultiplePurchases = value;
      }
    }
  }
  private GameSkill? _skill = null;
  public GameSkill? Skill
  {
    get => _skill;
    set
    {
      if (_skill != value)
      {
        _skill = value;
        _updated.Skill = new Change<GameSkill?>(value);
      }
    }
  }
  public TalentId? RequiredTalentId { get; private set; }

  public Talent() : base()
  {
  }

  public Talent(World world, Tier tier, Name name, UserId? userId = null, TalentId? talentId = null)
    : this(world.Id, tier, name, userId ?? world.OwnerId, talentId)
  {
  }
  public Talent(WorldId worldId, Tier tier, Name name, UserId userId, TalentId? talentId = null)
    : base((talentId ?? TalentId.NewId(worldId)).StreamId)
  {
    Raise(new TalentCreated(tier, name), userId.ActorId);
  }
  protected virtual void Handle(TalentCreated @event)
  {
    _tier = @event.Tier;
    _name = @event.Name;
  }

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0);

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new TalentDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void SetRequiredTalent(Talent? requiredTalent)
  {
    if (RequiredTalentId != requiredTalent?.Id)
    {
      if (requiredTalent is not null && requiredTalent.Tier.Value > Tier.Value)
      {
        throw new ArgumentException("", nameof(requiredTalent)); // TODO(fpion): message
      }
      RequiredTalentId = requiredTalent?.Id;
      _updated.RequiredTalentId = new Change<TalentId?>(requiredTalent?.Id);
    }
  }

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new TalentUpdated();
    }
  }
  protected virtual void Handle(TalentUpdated @event)
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

    if (@event.AllowMultiplePurchases is not null)
    {
      _allowMultiplePurchases = @event.AllowMultiplePurchases.Value;
    }
    if (@event.Skill is not null)
    {
      _skill = @event.Skill.Value;
    }
    if (@event.RequiredTalentId is not null)
    {
      RequiredTalentId = @event.RequiredTalentId.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
