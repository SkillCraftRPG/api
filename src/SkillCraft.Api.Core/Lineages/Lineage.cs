using Logitar.EventSourcing;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages;

public class Lineage : AggregateRoot, IResource
{
  public const string ResourceKind = "Lineage";

  public new LineageId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  public LineageId? ParentId { get; private set; }

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");
  public Summary? Summary { get; private set; }
  public Content? Content { get; private set; }

  private readonly List<Feature> _features = [];
  public IReadOnlyCollection<Feature> Features => _features.AsReadOnly();
  public LineageLanguages Languages { get; private set; } = new();
  public LineageNames Names { get; private set; } = new();
  public LineageSpeeds Speeds { get; private set; } = new();
  public LineageSize Size { get; private set; } = new();
  public LineageWeight Weight { get; private set; } = new();
  public LineageAge Age { get; private set; } = new();

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId.ResourceId);

  public Lineage() : base()
  {
  }

  public Lineage(World world, Name name, Lineage? parent = null, ActorId? actorId = null)
    : this(LineageId.NewId(world.Id), name, parent, actorId)
  {
  }

  public Lineage(LineageId LineageId, Name name, Lineage? parent = null, ActorId? actorId = null)
    : base(LineageId.StreamId)
  {
    if (parent is not null && parent.ParentId.HasValue)
    {
      throw new InvalidParentLineageException(parent, nameof(ParentId));
    }

    Raise(new LineageCreated(parent?.Id, name), actorId);
  }
  protected virtual void Handle(LineageCreated @event)
  {
    ParentId = @event.ParentId;

    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new LineageDeleted(), actorId);
    }
  }

  public void Edit(Summary? summary, Content? content, ActorId? actorId = null)
  {
    if (!Equals(Summary, summary) || !Equals(Content, content))
    {
      Raise(new LineageEdited(summary, content), actorId);
    }
  }
  protected virtual void Handle(LineageEdited @event)
  {
    Summary = @event.Summary;
    Content = @event.Content;
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new LineageRenamed(name), actorId);
    }
  }
  protected virtual void Handle(LineageRenamed @event)
  {
    _name = @event.Name;
  }

  public void SetFeatures(IEnumerable<Feature> features, ActorId? actorId = null)
  {
    IReadOnlyCollection<Feature> cleaned = features
      .GroupBy(feature => feature.Name)
      .Select(group => group.Last())
      .OrderBy(feature => feature.Name)
      .ToList()
      .AsReadOnly();

    if (!Features.SequenceEqual(cleaned))
    {
      Raise(new LineageFeaturesChanged(cleaned), actorId);
    }
  }
  protected virtual void Handle(LineageFeaturesChanged @event)
  {
    _features.Clear();
    _features.AddRange(@event.Features);
  }

  public void SetLanguages(LineageLanguages languages, ActorId? actorId = null)
  {
    foreach (LanguageId id in languages.Ids)
    {
      WorldMismatchException.ThrowIfMismatch(WorldId, id.WorldId, nameof(languages));
    }

    if (!Equals(Languages, languages))
    {
      Raise(new LineageLanguagesChanged(languages), actorId);
    }
  }
  protected virtual void Handle(LineageLanguagesChanged @event)
  {
    Languages = @event.Languages;
  }

  public void SetNames(LineageNames names, ActorId? actorId = null)
  {
    if (!Equals(Names, names))
    {
      Raise(new LineageNamesChanged(names), actorId);
    }
  }
  protected virtual void Handle(LineageNamesChanged @event)
  {
    Names = @event.Names;
  }

  public void SetSpeeds(LineageSpeeds speeds, ActorId? actorId = null)
  {
    if (!Equals(Speeds, speeds))
    {
      Raise(new LineageSpeedsChanged(speeds), actorId);
    }
  }
  protected virtual void Handle(LineageSpeedsChanged @event)
  {
    Speeds = @event.Speeds;
  }

  public void SetTraits(LineageSize size, LineageWeight weight, LineageAge age, ActorId? actorId = null)
  {
    if (!Equals(Size, size) || !Equals(Weight, weight) || !Equals(Age, age))
    {
      Raise(new LineageTraitsChanged(size, weight, age), actorId);
    }
  }
  protected virtual void Handle(LineageTraitsChanged @event)
  {
    Size = @event.Size;
    Weight = @event.Weight;
    Age = @event.Age;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
