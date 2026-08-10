using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters;
using SkillCraft.Api.Core.Characters.Events;
using SkillCraft.Api.Infrastructure.Compendium.Models;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class CharacterEntity : AggregateEntity
{
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

  public List<CustomizationEntity> Customizations { get; private set; } = [];
  public List<LanguageEntity> Languages { get; private set; } = [];
  public List<CharacterTalentEntity> Talents { get; private set; } = [];

  public CharacterEntity(
    Character character,
    int lineageId,
    int casteId,
    int educationId,
    IEnumerable<CustomizationEntity> customizations,
    IEnumerable<LanguageEntity> languages,
    IEnumerable<TalentEntity> talents)
  {
    WorldId = character.WorldId.ResourceId;
    Id = character.ResourceId;

    LineageId = lineageId;
    CasteId = casteId;
    EducationId = educationId;

    Attributes = EncodeAttributes(character.StartingAttributes);
    Skills = EncodeSkills(character.Skills);

    Customizations.Clear();
    Customizations.AddRange(customizations);

    Languages.Clear();
    Languages.AddRange(languages);

    SetTalents(character.Talents, talents);

    Update(character);
  }

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

    Attributes = EncodeAttributes(@event.Attributes);
    Skills = EncodeSkills(@event.Skills);

    Customizations.Clear();
    Customizations.AddRange(customizations);

    Languages.Clear();
    Languages.AddRange(languages);

    SetTalents(@event.Talents, talents);
  }

  private CharacterEntity() : base()
  {
  }

  public void Update(Character character)
  {
    base.Update(character);

    Name = character.Name.Value;
    DominantHand = character.DominantHand;

    SetAppearance(character.Appearance);
    Alignment = character.Alignment;
    SetPersonality(character.Personality);
    Background = character.Background?.Value;
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

  private void SetTalents(IReadOnlyDictionary<Guid, CharacterTalent> talents, IEnumerable<TalentEntity> entities)
  {
    Dictionary<string, TalentEntity> entitiesById = entities.ToDictionary(x => x.StreamId, x => x);

    Talents.Clear();
    foreach (KeyValuePair<Guid, CharacterTalent> talent in talents)
    {
      if (entitiesById.TryGetValue(talent.Value.TalentId.Value, out TalentEntity? entity))
      {
        Talents.Add(new CharacterTalentEntity(this, entity, talent.Value, talent.Key));
      }
      else
      {
        throw new InvalidOperationException($"The talent entity 'StreamId={talent.Value.TalentId}' was not found.");
      }
    }
  }

  private static string? EncodeAttributes(IStartingAttributes attributes)
  {
    Dictionary<GameAttribute, int> data = new(capacity: 5);
    if (attributes.Dexterity > 0)
    {
      data[GameAttribute.Dexterity] = attributes.Dexterity;
    }
    if (attributes.Health > 0)
    {
      data[GameAttribute.Health] = attributes.Health;
    }
    if (attributes.Intellect > 0)
    {
      data[GameAttribute.Intellect] = attributes.Intellect;
    }
    if (attributes.Senses > 0)
    {
      data[GameAttribute.Senses] = attributes.Senses;
    }
    if (attributes.Vigor > 0)
    {
      data[GameAttribute.Vigor] = attributes.Vigor;
    }
    return data.Count < 1 ? null : string.Join('|', data.Select(pair => string.Join(':', pair.Key, pair.Value)));
  }

  private static string? EncodeSkills(IReadOnlyDictionary<Skill, int> skills)
  {
    string encoded = string.Join('|', skills.Where(x => x.Value > 0).Select(pair => string.Join(':', pair.Key, pair.Value)));
    return string.IsNullOrWhiteSpace(encoded) ? null : encoded.Trim();
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
