using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Characters;

public class Character : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Character";

  public new CharacterId Id => new(base.Id);
  public WorldId WorldId => Id.WorldId;
  public Guid EntityId => Id.EntityId;

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The caste has not been initialized.");
  public Characteristics Characteristics { get; private set; } = new();

  public LineageId LineageId { get; private set; }
  public CasteId CasteId { get; private set; }
  public EducationId EducationId { get; private set; }

  private readonly HashSet<LanguageId> _languageIds = [];
  public IReadOnlyCollection<LanguageId> LanguageIds => _languageIds.ToList().AsReadOnly();

  private readonly HashSet<CustomizationId> _customizationIds = [];
  public IReadOnlyCollection<CustomizationId> CustomizationIdIds => _customizationIds.ToList().AsReadOnly();

  public Character() : base()
  {
  }

  public Character(
    WorldId worldId,
    Name name,
    Lineage lineage,
    Caste caste,
    Education education,
    UserId userId,
    Characteristics? characteristics = null,
    IEnumerable<Language>? languages = null,
    IEnumerable<Customization>? customizations = null,
    CharacterId? characterId = null) : base((characterId ?? CharacterId.NewId(worldId)).StreamId)
  {
    if (lineage.WorldId != worldId)
    {
      throw new ArgumentException($"The lineage (WorldId={lineage.WorldId}) and character (WorldId={worldId}) should be in the same world.", nameof(lineage));
    }
    if (caste.WorldId != worldId)
    {
      throw new ArgumentException($"The caste (WorldId={caste.WorldId}) and character (WorldId={worldId}) should be in the same world.", nameof(caste));
    }
    if (education.WorldId != worldId)
    {
      throw new ArgumentException($"The education (WorldId={education.WorldId}) and character (WorldId={worldId}) should be in the same world.", nameof(education));
    }

    languages ??= [];
    if (languages.Any(languageId => languageId.WorldId != worldId))
    {
      throw new ArgumentException($"All languages should be in the same world (Id={worldId}) as the character.", nameof(languages));
    }
    // TODO(fpion): check that languages do not intersect with lineage.Languages.Ids
    HashSet<LanguageId> languageIds = languages.Select(language => language.Id).ToHashSet();

    customizations ??= [];
    if (customizations.Any(customizationId => customizationId.WorldId != worldId))
    {
      throw new ArgumentException($"All customizations should be in the same world (Id={worldId}) as the character.", nameof(customizations));
    }
    // TODO(fpion): check that number of Gifts == number of Disabilities
    HashSet<CustomizationId> customizationIds = customizations.Select(customization => customization.Id).ToHashSet();

    characteristics ??= new();
    Raise(new CharacterCreated(name, characteristics, lineage.Id, caste.Id, education.Id, languageIds, customizationIds), userId.ActorId);
  }
  protected virtual void Handle(CharacterCreated @event)
  {
    _name = @event.Name;
    Characteristics = @event.Characteristics;

    LineageId = @event.LineageId;
    CasteId = @event.CasteId;
    EducationId = @event.EducationId;

    _languageIds.Clear();
    _languageIds.AddRange(@event.LanguageIds);

    _customizationIds.Clear();
    _customizationIds.AddRange(@event.CustomizationIds);
  }

  public long CalculateSize() => Name.Size + Characteristics.Size;

  public void Delete(UserId userId)
  {
    if (!IsDeleted)
    {
      Raise(new CharacterDeleted(), userId.ActorId);
    }
  }

  public Entity GetEntity() => new(EntityKind, EntityId, WorldId, CalculateSize());

  public bool HasCustomization(Customization customization) => HasCustomization(customization.Id);
  public bool HasCustomization(CustomizationId customizationId) => _customizationIds.Contains(customizationId);

  public bool HasLanguage(Language language) => HasLanguage(language.Id);
  public bool HasLanguage(LanguageId languageId) => _languageIds.Contains(languageId);

  public override string ToString() => $"{Name} | {base.ToString()}";
}
