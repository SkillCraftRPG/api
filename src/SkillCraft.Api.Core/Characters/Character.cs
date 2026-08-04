using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Characters;

public class Character : AggregateRoot, IResource
{
  public const string ResourceKind = "Character";

  public new CharacterId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid ResourceId => Id.ResourceId;

  public LineageId LineageId { get; private set; }

  private readonly List<LanguageId> _languageIds = [];
  public IReadOnlyCollection<LanguageId> LanguageIds => _languageIds.AsReadOnly();

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Character() : base()
  {
  }

  public Character(World world, Lineage lineage, IEnumerable<Language> languages, ActorId? actorId = null)
    : this(CharacterId.NewId(world.Id), lineage, languages, actorId)
  {
  }

  public Character(CharacterId characterId, Lineage lineage, IEnumerable<Language> languages, ActorId? actorId = null)
    : base(characterId.StreamId)
  {
    WorldMismatchException.ThrowIfMismatch(WorldId, lineage.WorldId, nameof(lineage));
    foreach (Language language in languages)
    {
      WorldMismatchException.ThrowIfMismatch(WorldId, language.WorldId, nameof(languages));
    }

    Raise(new CharacterCreated(lineage.Id, languages.Select(language => language.Id).Distinct().ToList().AsReadOnly()), actorId);
  }
  protected virtual void Handle(CharacterCreated @event)
  {
    LineageId = @event.LineageId;

    _languageIds.Clear();
    _languageIds.AddRange(@event.LanguageIds);
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new CharacterDeleted(), actorId);
    }
  }

  // TODO(fpion): ToString
}
