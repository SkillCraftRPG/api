using Logitar.EventSourcing;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Core.Customizations;
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

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name has not been initialized.");

  public LineageId LineageId { get; private set; }

  private readonly List<CustomizationId> _customizationIds = [];
  public IReadOnlyCollection<CustomizationId> CustomizationIds => _customizationIds.AsReadOnly();

  private readonly List<LanguageId> _languageIds = [];
  public IReadOnlyCollection<LanguageId> LanguageIds => _languageIds.AsReadOnly();

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Character() : base()
  {
  }

  public Character(
    World world,
    Name name,
    Lineage lineage,
    IEnumerable<Language>? languages = null,
    IEnumerable<Customization>? customizations = null,
    ActorId? actorId = null) : this(CharacterId.NewId(world.Id), name, lineage, customizations, languages, actorId)
  {
  }

  public Character(
    CharacterId characterId,
    Name name,
    Lineage lineage,
    IEnumerable<Customization>? customizations = null,
    IEnumerable<Language>? languages = null,
    ActorId? actorId = null) : base(characterId.StreamId)
  {
    WorldMismatchException.ThrowIfMismatch(WorldId, lineage.WorldId, nameof(lineage));

    if (customizations is not null)
    {
      int disabilities = 0;
      int gifts = 0;
      foreach (Customization customization in customizations)
      {
        WorldMismatchException.ThrowIfMismatch(WorldId, customization.WorldId, nameof(customizations));
        switch (customization.Kind)
        {
          case CustomizationKind.Disability:
            disabilities++;
            break;
          case CustomizationKind.Gift:
            gifts++;
            break;
        }
      }
      if (disabilities != gifts)
      {
        throw new NotImplementedException(); // TODO(fpion): implement
      }
    }

    if (languages is not null)
    {
      foreach (Language language in languages)
      {
        WorldMismatchException.ThrowIfMismatch(WorldId, language.WorldId, nameof(languages));
      }
    }

    HashSet<CustomizationId> customizationIds = (customizations ?? []).Select(customization => customization.Id).ToHashSet();
    HashSet<LanguageId> languageIds = (languages ?? []).Select(language => language.Id).ToHashSet();
    Raise(new CharacterCreated(name, lineage.Id, customizationIds, languageIds), actorId);
  }
  protected virtual void Handle(CharacterCreated @event)
  {
    _name = @event.Name;

    LineageId = @event.LineageId;

    _customizationIds.Clear();
    _customizationIds.AddRange(@event.CustomizationIds);

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

  public override string ToString() => $"{Name} | {base.ToString()}";
}
