using FluentValidation;
using FluentValidation.Results;
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
    || _updated.Requirements is not null || _updated.Options is not null;

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

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0) + Requirements.Size + Options.Size; // TODO(fpion): Doctrine

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new SpecializationDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void SetOptions(IEnumerable<Talent>? talents = null, IEnumerable<string>? other = null)
  {
    // TODO(fpion): implement
  }

  public void SetRequirements(Talent? talent = null, IEnumerable<string>? other = null)
  {
    if (talent is not null)
    {
      if (talent.WorldId != WorldId)
      {
        throw new ArgumentException($"The required talent (WorldId={talent.WorldId}) and specialization (WorldId={WorldId}) should be in the same world.", nameof(talent));
      }
      else if (talent.Tier.Value > Tier.Value)
      {
        ValidationFailure failure = new("Requirements.TalentId", "The required talent tier should be lower than or equal to the specialization tier.", talent.EntityId)
        {
          CustomState = new
          {
            Tier = Tier.Value,
            RequiredTalentTier = talent.Tier.Value
          },
          ErrorCode = "InvalidTalentRequirement"
        };
        throw new ValidationException([failure]); // TODO(fpion): custom exception
      }
    }

    Requirements requirements = new(talent?.Id, other?.ToArray() ?? []);
    if (Requirements != requirements) // TODO(fpion): probably won't work
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
    // TODO(fpion): Doctrine
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
