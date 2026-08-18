using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Talents;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CharacterEntity : AggregateEntity
{
  private const char Separator = '|';
  private const char PairSeparator = ':';

  public int CharacterId { get; private set; }

  public WorldEntity? World { get; private set; }
  public Guid WorldId { get; private set; }
  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public DominantHand? DominantHand { get; private set; }

  public LineageEntity? Lineage { get; private set; }
  public int LineageId { get; private set; }
  public CasteEntity? Caste { get; private set; }
  public int CasteId { get; private set; }
  public EducationEntity? Education { get; private set; }
  public int EducationId { get; private set; }

  public int? Height { get; private set; }
  public int? Weight { get; private set; }
  public int? Age { get; private set; }

  public string? Skin { get; private set; }
  public string? Eyes { get; private set; }
  public string? Hair { get; private set; }

  public Alignment? Alignment { get; private set; }

  public string? Traits { get; private set; }
  public string? Ideals { get; private set; }
  public string? Flaws { get; private set; }

  public string? Background { get; private set; }

  public string? Attributes { get; private set; }
  public string? Skills { get; private set; }

  public List<CharacterCustomizationEntity> Customizations { get; private set; } = [];
  public List<CharacterLanguageEntity> Languages { get; private set; } = [];
  public List<CharacterModifierEntity> Modifiers { get; private set; } = [];
  public List<CharacterTalentEntity> Talents { get; private set; } = [];

  public CharacterEntity(
    int lineageId,
    int casteId,
    int educationId,
    IEnumerable<CustomizationEntity> customizations,
    IEnumerable<LanguageEntity> languages,
    IEnumerable<TalentEntity> talents,
    CharacterCreated @event) : base(@event)
  {
    CharacterId characterId = new(@event.StreamId);
    WorldId = characterId.WorldId.ResourceId;
    Id = characterId.ResourceId;

    Name = @event.Name.Value;
    DominantHand = @event.DominantHand;

    LineageId = lineageId;
    CasteId = casteId;
    EducationId = educationId;

    SetAppearance(@event.Appearance);
    Alignment = @event.Alignment;
    SetPersonality(@event.Personality);
    Background = @event.Background?.Value;

    Attributes = new CharacterAttributesEntity(@event.Attributes).ToString();
    // TODO(fpion): should set current Vitality = max. Vitality
    // TODO(fpion): should set current Stamina = max. Stamina
    SetSkillRanks(@event.Skills);

    Dictionary<CustomizationId, CustomizationEntity> customizationsById = customizations.ToDictionary(x => new CustomizationId(x.StreamId), x => x);
    foreach (CustomizationId customizationId in @event.CustomizationIds)
    {
      CustomizationEntity customization = customizationsById.GetValueOrDefault(customizationId)
        ?? throw new ArgumentException($"The customization entity 'StreamId={customizationId}' was not found.", nameof(customizations));
      Customizations.Add(new CharacterCustomizationEntity(this, customization));
    }

    Dictionary<LanguageId, LanguageEntity> languagesById = languages.ToDictionary(x => new LanguageId(x.StreamId), x => x);
    foreach (LanguageId languageId in @event.LanguageIds)
    {
      LanguageEntity language = languagesById.GetValueOrDefault(languageId)
        ?? throw new ArgumentException($"The language entity 'StreamId={languageId}' was not found.", nameof(languages));
      Languages.Add(new CharacterLanguageEntity(this, language, @event));
    }

    Dictionary<TalentId, TalentEntity> talentsById = talents.ToDictionary(x => new TalentId(x.StreamId), x => x);
    foreach (KeyValuePair<Guid, CharacterTalent> acquisition in @event.Talents)
    {
      TalentEntity talent = talentsById.GetValueOrDefault(acquisition.Value.TalentId)
        ?? throw new ArgumentException($"The talent entity 'StreamId={acquisition.Value.TalentId}' was not found.", nameof(talents));
      Talents.Add(new CharacterTalentEntity(this, talent, acquisition.Value, @event, acquisition.Key));
    }
  }

  private CharacterEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (Lineage is not null)
    {
      actorIds.AddRange(Lineage.GetActorIds());
    }
    if (Caste is not null)
    {
      actorIds.AddRange(Caste.GetActorIds());
    }
    if (Education is not null)
    {
      actorIds.AddRange(Education.GetActorIds());
    }
    foreach (CharacterCustomizationEntity customization in Customizations)
    {
      if (customization.Customization is not null)
      {
        actorIds.AddRange(customization.Customization.GetActorIds());
      }
    }
    foreach (CharacterLanguageEntity language in Languages)
    {
      actorIds.AddRange(language.GetActorIds());
    }
    foreach (CharacterModifierEntity modifier in Modifiers)
    {
      actorIds.AddRange(modifier.GetActorIds());
    }
    foreach (CharacterTalentEntity talent in Talents)
    {
      actorIds.AddRange(talent.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void AddCustomization(CustomizationEntity customization, CharacterCustomizationAdded @event)
  {
    base.Update(@event);

    Customizations.Add(new CharacterCustomizationEntity(this, customization));
  }

  public void RemoveCustomization(CharacterCustomizationRemoved @event)
  {
    base.Update(@event);

    CharacterCustomizationEntity? customization = Customizations
      .SingleOrDefault(x => x.Customization?.StreamId == @event.CustomizationId.Value);
    if (customization is not null)
    {
      Customizations.Remove(customization);
    }
  }

  public void RemoveLanguage(CharacterLanguageRemoved @event)
  {
    base.Update(@event);

    CharacterLanguageEntity? language = TryGetLanguage(@event.LanguageId);
    if (language is not null)
    {
      Languages.Remove(language);
    }
  }

  public void RemoveModifier(CharacterModifierRemoved @event)
  {
    base.Update(@event);

    CharacterModifierEntity? modifier = TryGetModifier(@event.ModifierId);
    if (modifier is not null)
    {
      Modifiers.Remove(modifier);
    }
  }

  public void Rename(CharacterRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetModifier(CharacterModifierChanged @event)
  {
    base.Update(@event);

    CharacterModifierEntity? modifier = TryGetModifier(@event.ModifierId);
    if (modifier is null)
    {
      modifier = new CharacterModifierEntity(this, @event);
      Modifiers.Add(modifier);
    }
    else
    {
      modifier.Update(@event);
    }
  }

  public void SetProfile(CharacterProfileChanged @event)
  {
    base.Update(@event);

    DominantHand = @event.DominantHand;

    SetAppearance(@event.Appearance);
    Alignment = @event.Alignment;
    SetPersonality(@event.Personality);
    Background = @event.Background?.Value;
  }

  private void SetAppearance(ICharacterAppearance appearance)
  {
    Height = appearance.Height;
    Weight = appearance.Weight;
    Age = appearance.Age;

    Skin = appearance.Skin;
    Eyes = appearance.Eyes;
    Hair = appearance.Hair;
  }

  private void SetPersonality(ICharacterPersonality personality)
  {
    Traits = personality.Traits;
    Ideals = personality.Ideals;
    Flaws = personality.Flaws;
  }

  public IReadOnlyDictionary<Skill, int> GetSkillRanks()
  {
    Dictionary<Skill, int> skillRanks = new(capacity: 20);
    if (Skills is not null)
    {
      string[] values = Skills.Split(Separator);
      foreach (string value in values)
      {
        string[] parts = value.Split(PairSeparator);
        if (parts.Length == 2 && Enum.TryParse(parts[0], out Skill skill) && Enum.IsDefined(skill) && int.TryParse(parts[1], out int rank))
        {
          skillRanks[skill] = rank;
        }
      }
    }
    return skillRanks.AsReadOnly();
  }
  private void SetSkillRanks(IReadOnlyDictionary<Skill, int> skills)
  {
    string encoded = string.Join(Separator, skills.Where(x => x.Value != 0).Select(pair => string.Join(PairSeparator, pair.Key, pair.Value)));
    Skills = string.IsNullOrEmpty(encoded) ? null : encoded;
  }

  private CharacterLanguageEntity? TryGetLanguage(LanguageId languageId) =>
    Languages.SingleOrDefault(language => language.Language?.StreamId == languageId.Value);

  private CharacterModifierEntity? TryGetModifier(Guid id) => Modifiers.SingleOrDefault(modifier => modifier.Id == id);

  public override string ToString() => $"{Name} | {base.ToString()}";
}
