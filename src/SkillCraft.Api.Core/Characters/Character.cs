using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Items;
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

  private readonly HashSet<CustomizationId> _customizationIds = [];
  public IReadOnlyCollection<CustomizationId> CustomizationIds => _customizationIds.AsReadOnly();

  private readonly List<LanguageId> _languageIds = [];
  public IReadOnlyCollection<LanguageId> LanguageIds => _languageIds.AsReadOnly();

  private readonly Dictionary<Guid, CharacterTalent> _talents = [];
  public IReadOnlyDictionary<Guid, CharacterTalent> Talents => _talents.AsReadOnly();

  public StartingAttributes StartingAttributes { get; private set; } = new();
  private readonly Dictionary<Skill, int> _skills = [];
  public IReadOnlyDictionary<Skill, int> Skills => _skills.AsReadOnly();

  public CharacterAppearance Appearance { get; private set; } = new();

  public Alignment? Alignment { get; private set; }
  public CharacterPersonality Personality { get; private set; } = new();

  public Background? Background { get; private set; }

  private readonly Dictionary<ItemId, int> _inventory = [];
  public IReadOnlyDictionary<ItemId, int> Inventory => _inventory.AsReadOnly();

  private readonly Dictionary<Guid, CharacterModifier> _modifiers = [];
  public IReadOnlyDictionary<Guid, CharacterModifier> Modifiers => _modifiers.AsReadOnly();

  public ResourceIdentifier Identifier => new(ResourceKind, ResourceId, WorldId);

  public Character() : base()
  {
  }

  public Character(
    CharacterId characterId,
    Lineage lineage,
    Name name,
    Caste caste,
    Education education,
    Lineage? parent = null,
    IEnumerable<Language>? languages = null,
    DominantHand? dominantHand = null,
    IEnumerable<Customization>? customizations = null,
    IEnumerable<CharacterTalent>? talents = null,
    StartingAttributes? attributes = null,
    IReadOnlyDictionary<Skill, int>? skills = null,
    CharacterAppearance? appearance = null,
    Alignment? alignment = null,
    CharacterPersonality? personality = null,
    Background? background = null,
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

    attributes ??= new();

    skills ??= new Dictionary<Skill, int>().AsReadOnly();
    CharacterHelper.ValidateSkills(skills, characterTalents.Values.Select(acquired => acquired.Talent!));

    appearance ??= new();

    if (alignment.HasValue && !Enum.IsDefined(alignment.Value))
    {
      throw new ArgumentOutOfRangeException(nameof(alignment));
    }
    personality ??= new();

    Raise(new CharacterCreated(
      lineage.Id,
      languageIds,
      name,
      dominantHand,
      customizationIds,
      caste.Id,
      education.Id,
      characterTalents,
      attributes,
      skills,
      appearance,
      alignment,
      personality,
      background), actorId);
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

    StartingAttributes = @event.Attributes;
    _skills.Clear();
    foreach (KeyValuePair<Skill, int> skill in @event.Skills)
    {
      _skills[skill.Key] = skill.Value;
    }

    Appearance = @event.Appearance;

    Alignment = @event.Alignment;
    Personality = @event.Personality;

    Background = @event.Background;
  }

  public void Add(Item item, int quantity, ActorId? actorId = null)
  {
    WorldMismatchException.ThrowIfMismatch(WorldId, item.WorldId, nameof(item));
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity, nameof(quantity));

    Raise(new CharacterInventoryAdded(item.Id, quantity), actorId);
  }
  protected virtual void Handle(CharacterInventoryAdded @event)
  {
    int quantity = _inventory.GetValueOrDefault(@event.ItemId);
    _inventory[@event.ItemId] = quantity + @event.Quantity;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new CharacterDeleted(), actorId);
    }
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Equals(Name, name))
    {
      Raise(new CharacterRenamed(name), actorId);
    }
  }
  protected virtual void Handle(CharacterRenamed @event)
  {
    _name = @event.Name;
  }

  public void SetProfile(
    DominantHand? dominantHand,
    CharacterAppearance appearance,
    Alignment? alignment,
    CharacterPersonality personality,
    Background? background,
    ActorId? actorId = null)
  {
    if (!Equals(DominantHand, dominantHand) || !Equals(Appearance, appearance) || !Equals(Alignment, alignment) || !Equals(Personality, personality) || !Equals(Background, background))
    {
      Raise(new CharacterProfileChanged(dominantHand, appearance, alignment, personality, background), actorId);
    }
  }
  protected virtual void Handle(CharacterProfileChanged @event)
  {
    DominantHand = @event.DominantHand;

    Appearance = @event.Appearance;
    Alignment = @event.Alignment;
    Personality = @event.Personality;
    Background = @event.Background;
  }

  #region Customizations
  public void AddCustomization(Customization customization, ActorId? actorId = null) => AddCustomization(customization.Id, actorId);
  public void AddCustomization(CustomizationId customizationId, ActorId? actorId = null)
  {
    WorldMismatchException.ThrowIfMismatch(WorldId, customizationId.WorldId, nameof(customizationId));

    if (!HasCustomization(customizationId))
    {
      Raise(new CharacterCustomizationAdded(customizationId), actorId);
    }
  }
  protected virtual void Handle(CharacterCustomizationAdded @event)
  {
    _customizationIds.Add(@event.CustomizationId);
  }

  public bool HasCustomization(Customization customization) => HasCustomization(customization.Id);
  public bool HasCustomization(CustomizationId customizationId) => _customizationIds.Contains(customizationId);

  public void RemoveCustomization(Customization customization, ActorId? actorId = null) => RemoveCustomization(customization.Id, actorId);
  public void RemoveCustomization(CustomizationId customizationId, ActorId? actorId = null)
  {
    WorldMismatchException.ThrowIfMismatch(WorldId, customizationId.WorldId, nameof(customizationId));

    if (HasCustomization(customizationId))
    {
      Raise(new CharacterCustomizationRemoved(customizationId), actorId);
    }
  }
  protected virtual void Handle(CharacterCustomizationRemoved @event)
  {
    _customizationIds.Remove(@event.CustomizationId);
  }
  #endregion

  #region Modifiers
  public void AddModifier(CharacterModifier modifier, ActorId? actorId = null) => SetModifier(Guid.NewGuid(), modifier, actorId);

  public CharacterModifier FindModifier(Guid id) => TryGetModifier(id) ?? throw new ArgumentException($"The modifier 'Id={id}' was not found.", nameof(id));

  public bool HasModifier(Guid id) => _modifiers.ContainsKey(id);

  public void RemoveModifier(Guid id, ActorId? actorId = null)
  {
    if (HasModifier(id))
    {
      Raise(new CharacterModifierRemoved(id), actorId);
    }
  }
  protected virtual void Handle(CharacterModifierRemoved @event)
  {
    _modifiers.Remove(@event.ModifierId);
  }

  public void SetModifier(Guid id, CharacterModifier modifier, ActorId? actorId = null)
  {
    if (!_modifiers.TryGetValue(id, out CharacterModifier? existingModifier) || !existingModifier.Equals(modifier))
    {
      Raise(new CharacterModifierChanged(id, modifier), actorId);
    }
  }
  protected virtual void Handle(CharacterModifierChanged @event)
  {
    _modifiers[@event.ModifierId] = @event.Modifier;
  }

  public CharacterModifier? TryGetModifier(Guid id) => _modifiers.GetValueOrDefault(id);
  #endregion

  public override string ToString() => $"{Name} | {base.ToString()}";
}
