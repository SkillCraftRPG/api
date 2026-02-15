using Logitar.EventSourcing;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Educations;

public class Education : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Education";

  private EducationUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Summary is not null || _updated.Description is not null
    || _updated.Skill is not null || _updated.WealthMultiplier is not null || _updated.Feature is not null;

  public new EducationId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException("The education has not been initialized.");
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
  private WealthMultiplier? _wealthMultiplier = null;
  public WealthMultiplier? WealthMultiplier
  {
    get => _wealthMultiplier;
    set
    {
      if (_wealthMultiplier != value)
      {
        _wealthMultiplier = value;
        _updated.WealthMultiplier = new Change<WealthMultiplier>(value);
      }
    }
  }
  private Feature? _feature = null;
  public Feature? Feature
  {
    get => _feature;
    set
    {
      if (_feature != value)
      {
        _feature = value;
        _updated.Feature = new Change<Feature>(value);
      }
    }
  }

  public Education() : base()
  {
  }

  public Education(World world, Name name, UserId? userId = null, EducationId? educationId = null)
    : this(world.Id, name, userId ?? world.OwnerId, educationId)
  {
  }
  public Education(WorldId worldId, Name name, UserId userId, EducationId? educationId = null)
    : base((educationId ?? EducationId.NewId(worldId)).StreamId)
  {
    Raise(new EducationCreated(name), userId.ActorId);
  }
  protected virtual void Handle(EducationCreated @event)
  {
    _name = @event.Name;
  }

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0) + (Feature?.Size ?? 0);

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new EducationDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new EducationUpdated();
    }
  }
  protected virtual void Handle(EducationUpdated @event)
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

    if (@event.Skill is not null)
    {
      _skill = @event.Skill.Value;
    }
    if (@event.WealthMultiplier is not null)
    {
      _wealthMultiplier = @event.WealthMultiplier.Value;
    }
    if (@event.Feature is not null)
    {
      _feature = @event.Feature.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
