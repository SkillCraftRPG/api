using Logitar.EventSourcing;
using SkillCraft.Api.Core.Specializations.Events;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Specializations;

public class Specialization : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Specialization";

  private SpecializationUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Summary is not null || _updated.Description is not null
    || _updated.Requirements is not null || _updated.Options is not null || _updated.Doctrine is not null;

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

  public Requirements Requirements { get; private set; } = new();
  public Options Options { get; private set; } = new();
  public Doctrine? Doctrine { get; private set; }

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

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0) + Requirements.Size + Options.Size + (Doctrine?.Size ?? 0);

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new SpecializationDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void RemoveDoctrine()
  {
    if (Doctrine is not null)
    {
      Doctrine = null;
      _updated.Doctrine = new Change<Doctrine>(null);
    }
  }

  public void SetDoctrine(Name name, IEnumerable<string> description, IEnumerable<Talent> discountedTalents, IEnumerable<Feature> features)
  {
    if (discountedTalents.Any(talent => talent.WorldId != WorldId))
    {
      throw new ArgumentException($"All talents should be in the same world (Id={WorldId}) as the specialization.", nameof(discountedTalents));
    }

    Doctrine doctrine = new(name, description, discountedTalents, features);
    if (Doctrine is null || !Doctrine.Equals(doctrine))
    {
      Doctrine = doctrine;
      _updated.Doctrine = new Change<Doctrine>(doctrine);
    }
  }

  public void SetOptions(IEnumerable<Talent> talents, IEnumerable<string> other)
  {
    if (talents.Any(talent => talent.WorldId != WorldId))
    {
      throw new ArgumentException($"All talents should be in the same world (Id={WorldId}) as the specialization.", nameof(talents));
    }

    IEnumerable<Talent> invalidTalents = talents.Where(talent => talent.Tier.Value >= Tier.Value);
    if (invalidTalents.Any())
    {
      string propertyName = string.Join('.', nameof(Options), nameof(Options.TalentIds));
      throw new InvalidSpecializationOptionsException(this, invalidTalents, propertyName);
    }

    Options options = new(talents.Select(talent => talent.Id).ToArray(), other.ToArray());
    if (!Options.Equals(options))
    {
      Options = options;
      _updated.Options = options;
    }
  }

  public void SetRequirements(Talent? talent, IEnumerable<string> other)
  {
    if (talent is not null)
    {
      if (talent.WorldId != WorldId)
      {
        throw new ArgumentException($"The required talent (WorldId={talent.WorldId}) and specialization (WorldId={WorldId}) should be in the same world.", nameof(talent));
      }
      else if (talent.Tier.Value >= Tier.Value)
      {
        string propertyName = string.Join('.', nameof(Requirements), nameof(Requirements.TalentId));
        throw new InvalidSpecializationRequirementException(this, talent, propertyName);
      }
    }

    Requirements requirements = new(talent?.Id, other?.ToArray() ?? []);
    if (!Requirements.Equals(requirements))
    {
      Requirements = requirements;
      _updated.Requirements = requirements;
    }
  }

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

    if (@event.Requirements is not null)
    {
      Requirements = @event.Requirements;
    }
    if (@event.Options is not null)
    {
      Options = @event.Options;
    }
    if (@event.Doctrine is not null)
    {
      Doctrine = @event.Doctrine.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
