using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
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
  public DominantHand? DominantHand { get; private set; }

  public LineageId LineageId { get; private set; }
  public CasteId CasteId { get; private set; }
  public EducationId EducationId { get; private set; }

  private readonly List<CustomizationId> _customizationIds = [];
  public IReadOnlyCollection<CustomizationId> CustomizationIds => _customizationIds.AsReadOnly();

  private readonly List<LanguageId> _languageIds = [];
  public IReadOnlyCollection<LanguageId> LanguageIds => _languageIds.AsReadOnly();

  private readonly Dictionary<Guid, CharacterTalent> _talents = [];
  public IReadOnlyDictionary<Guid, CharacterTalent> Talents => _talents.AsReadOnly();

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Character() : base()
  {
  }

  public Character(
    World world,
    Name name,
    StartingAttributes attributes,
    Lineage lineage,
    Caste caste,
    Education education,
    DominantHand? dominantHand = null,
    Lineage? parent = null,
    IEnumerable<Language>? languages = null,
    IEnumerable<Customization>? customizations = null,
    IEnumerable<CharacterTalent>? talents = null,
    ActorId? actorId = null)
    : this(CharacterId.NewId(world.Id), name, attributes, lineage, caste, education, dominantHand, parent, customizations, languages, talents, actorId)
  {
  }

  public Character(
    CharacterId characterId,
    Name name,
    StartingAttributes attributes,
    Lineage lineage,
    Caste caste,
    Education education,
    DominantHand? dominantHand = null,
    Lineage? parent = null,
    IEnumerable<Customization>? customizations = null,
    IEnumerable<Language>? languages = null,
    IEnumerable<CharacterTalent>? talents = null,
    ActorId? actorId = null) : base(characterId.StreamId)
  {
    CharacterHelper.ValidateLineage(WorldId, lineage, parent, nameof(lineage));
    WorldMismatchException.ThrowIfMismatch(WorldId, caste.WorldId, nameof(caste));
    WorldMismatchException.ThrowIfMismatch(WorldId, education.WorldId, nameof(education));

    if (dominantHand.HasValue && !Enum.IsDefined(dominantHand.Value))
    {
      throw new ArgumentOutOfRangeException(nameof(dominantHand));
    }

    IReadOnlyCollection<CustomizationId> customizationIds = CharacterHelper.ValidateCustomizations(WorldId, customizations ?? [], nameof(customizations));
    IReadOnlyCollection<LanguageId> languageIds = CharacterHelper.ValidateLanguages(WorldId, languages ?? [], lineage, parent, nameof(languages));
    IReadOnlyDictionary<Guid, CharacterTalent> characterTalents = CharacterHelper.ValidateTalents(
      WorldId,
      talents ?? [],
      lineage,
      parent,
      caste,
      education,
      customizations ?? [],
      nameof(talents));

    Raise(new CharacterCreated(name, dominantHand, attributes, lineage.Id, caste.Id, education.Id, customizationIds, languageIds, characterTalents), actorId);
  }
  protected virtual void Handle(CharacterCreated @event)
  {
    _name = @event.Name;
    DominantHand = @event.DominantHand;

    LineageId = @event.LineageId;
    CasteId = @event.CasteId;
    EducationId = @event.EducationId;

    _customizationIds.Clear();
    _customizationIds.AddRange(@event.CustomizationIds);

    _languageIds.Clear();
    _languageIds.AddRange(@event.LanguageIds);

    _talents.Clear();
    foreach (KeyValuePair<Guid, CharacterTalent> talent in @event.Talents)
    {
      _talents[talent.Key] = talent.Value;
    }
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
