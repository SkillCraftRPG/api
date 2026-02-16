using Logitar.EventSourcing;
using SkillCraft.Api.Core.Languages.Events;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Languages;

public class Language : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Language";

  private LanguageUpdated _updated = new();
  private bool HasUpdates => _updated.Name is not null || _updated.Summary is not null || _updated.Description is not null || _updated.TypicalSpeakers is not null;

  public new LanguageId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Name? _name = null;
  public Name Name
  {
    get => _name ?? throw new InvalidOperationException("The language has not been initialized.");
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

  public ScriptId? ScriptId { get; private set; }
  private TypicalSpeakers? _typicalSpeakers = null;
  public TypicalSpeakers? TypicalSpeakers
  {
    get => _typicalSpeakers;
    set
    {
      if (_typicalSpeakers != value)
      {
        _typicalSpeakers = value;
        _updated.TypicalSpeakers = new Change<TypicalSpeakers>(value);
      }
    }
  }

  public Language() : base()
  {
  }

  public Language(World world, Name name, UserId? userId = null, LanguageId? languageId = null)
    : this(world.Id, name, userId ?? world.OwnerId, languageId)
  {
  }
  public Language(WorldId worldId, Name name, UserId userId, LanguageId? languageId = null)
    : base((languageId ?? LanguageId.NewId(worldId)).StreamId)
  {
    Raise(new LanguageCreated(name), userId.ActorId);
  }
  protected virtual void Handle(LanguageCreated @event)
  {
    _name = @event.Name;
  }

  public long CalculateSize() => Name.Size + (Summary?.Size ?? 0) + (Description?.Size ?? 0) + (TypicalSpeakers?.Size ?? 0);

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new LanguageDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public void SetScript(Script? script)
  {
    if (ScriptId != script?.Id)
    {
      ScriptId = script?.Id;
      _updated.ScriptId = new Change<ScriptId?>(script?.Id);
    }
  }

  public void Update(UserId userId)
  {
    if (HasUpdates)
    {
      Raise(_updated, userId.ActorId, DateTime.Now);
      _updated = new LanguageUpdated();
    }
  }
  protected virtual void Handle(LanguageUpdated @event)
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

    if (@event.ScriptId is not null)
    {
      ScriptId = @event.ScriptId.Value;
    }
    if (@event.TypicalSpeakers is not null)
    {
      _typicalSpeakers = @event.TypicalSpeakers.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
