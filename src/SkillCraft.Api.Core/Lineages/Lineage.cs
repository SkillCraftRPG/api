using FluentValidation;
using FluentValidation.Results;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages;

public class Lineage : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Lineage";

  private LineageUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Summary is not null || _updated.Description is not null
    || _updated.Languages is not null || _updated.Names is not null
    || _updated.Speeds is not null || _updated.Size is not null || _updated.Weight is not null || _updated.Age is not null;

  public new LineageId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  public LineageId? ParentId { get; private set; }

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException("The lineage has not been initialized.");
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

  private LineageLanguages _languages = new();
  public LineageLanguages Languages
  {
    get => _languages;
    set
    {
      if (_languages != value)
      {
        if (value.Ids.Any(languageId => languageId.WorldId != WorldId))
        {
          throw new ArgumentException($"All languages should be in the same world (Id={WorldId}) as the lineage.", nameof(Languages));
        }

        _languages = value;
        _updated.Languages = value;
      }
    }
  }
  private Names _names = new();
  public Names Names
  {
    get => _names;
    set
    {
      if (_names != value)
      {
        _names = value;
        _updated.Names = value;
      }
    }
  }

  private Speeds _speeds = new();
  public Speeds Speeds
  {
    get => _speeds;
    set
    {
      if (_speeds != value)
      {
        _speeds = value;
        _updated.Speeds = value;
      }
    }
  }
  private Size _size = new();
  public Size Size
  {
    get => _size;
    set
    {
      if (_size != value)
      {
        _size = value;
        _updated.Size = value;
      }
    }
  }
  private Weight _weight = new();
  public Weight Weight
  {
    get => _weight;
    set
    {
      if (_weight != value)
      {
        _weight = value;
        _updated.Weight = value;
      }
    }
  }
  private Age _age = new();
  public Age Age
  {
    get => _age;
    set
    {
      if (_age != value)
      {
        _age = value;
        _updated.Age = value;
      }
    }
  }

  public Lineage() : base()
  {
  }

  public Lineage(World world, Name name, Lineage? parent, UserId userId, LineageId? lineageId = null)
    : this(world.Id, name, parent, userId, lineageId)
  {
  }
  public Lineage(WorldId worldId, Name name, Lineage? parent, UserId userId, LineageId? lineageId = null)
    : base((lineageId ?? LineageId.NewId(worldId)).StreamId)
  {
    if (parent is not null)
    {
      if (parent.WorldId != worldId)
      {
        throw new ArgumentException($"The parent (WorldId={parent.WorldId}) and child (WorldId={worldId}) lineages should be in the same world.", nameof(parent));
      }
      else if (parent.ParentId.HasValue)
      {
        ValidationFailure failure = new(nameof(ParentId), "The parent lineage should not have a parent.", parent.EntityId)
        {
          CustomState = new
          {
            ParentId = parent.ParentId.Value.EntityId
          },
          ErrorCode = "InvalidLineageParent"
        };
        throw new ValidationException([failure]);
      }
    }

    Raise(new LineageCreated(parent?.Id, name), userId.ActorId);
  }
  protected virtual void Handle(LineageCreated @event)
  {
    ParentId = @event.ParentId;

    _name = @event.Name;
  }

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0) + Languages.Size + Names.Size + (Size.Height?.Size ?? 0) + Weight.Size; // TODO(fpion): Features

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new LineageDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new LineageUpdated();
    }
  }
  protected virtual void Handle(LineageUpdated @event)
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

    if (@event.Languages is not null)
    {
      _languages = @event.Languages;
    }
    if (@event.Names is not null)
    {
      _names = @event.Names;
    }

    if (@event.Speeds is not null)
    {
      _speeds = @event.Speeds;
    }
    if (@event.Size is not null)
    {
      _size = @event.Size;
    }
    if (@event.Weight is not null)
    {
      _weight = @event.Weight;
    }
    if (@event.Age is not null)
    {
      _age = @event.Age;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
