using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Characters.Models;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

internal class CharacterMapper : Mapper
{
  public CharacterMapper() : base()
  {
  }

  public CharacterMapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors) : base(actors)
  {
  }

  public CharacterModel ToCharacter(CharacterEntity source)
  {
    LineageModel lineage = ToLineage(source.Lineage ?? throw new ArgumentException("The lineage is required.", nameof(source)));
    CasteEntity caste = source.Caste ?? throw new ArgumentException("The caste is required.", nameof(source));
    EducationEntity education = source.Education ?? throw new ArgumentException("The education is required.", nameof(source));

    CharacterModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      DominantHand = source.DominantHand,
      Tier = 0, // TODO(fpion): implement
      Level = 0, // TODO(fpion): implement
      Experience = 0, // TODO(fpion): implement
      Lineage = lineage,
      Caste = ToCaste(caste),
      Education = ToEducation(education),
      Appearance = GetAppearance(source),
      Alignment = source.Alignment,
      Personality = GetPersonality(source),
      Background = source.Background,
      Vitality = 0, // TODO(fpion): implement
      Stamina = 0, // TODO(fpion): implement
      BloodAlcoholContent = 0, // TODO(fpion): implement
      Intoxication = 0, // TODO(fpion): implement
      Hope = 0 // TODO(fpion): implement
    };

    foreach (CharacterCustomizationEntity entity in source.Customizations)
    {
      CustomizationEntity customization = entity.Customization ?? throw new ArgumentException("The customization is required.", nameof(source));
      destination.Customizations.Add(ToCustomization(customization));
    }

    foreach (CharacterLanguageEntity entity in source.Languages)
    {
      destination.Languages.Add(ToCharacterLanguage(entity));
    }

    foreach (CharacterTalentEntity entity in source.Talents)
    {
      destination.Talents.Add(ToCharacterTalent(entity));
    }

    CalculateAttributes(source, destination);
    CalculateStatistics(destination);
    CalculateSkills(source, destination);
    CalculateSpeeds(destination);
    CalculatePoints(destination);

    MapAggregate(source, destination);

    return destination;
  }

  private CharacterLanguageModel ToCharacterLanguage(CharacterLanguageEntity source)
  {
    LanguageEntity language = source.Language ?? throw new ArgumentException("The language is required.", nameof(source));
    return new CharacterLanguageModel
    {
      Language = ToLanguage(language),
      Source = source.Source,
      Target = source.Target,
      Notes = source.Notes,
      CreatedBy = FindActor(source.CreatedBy),
      CreatedOn = source.CreatedOn.AsUniversalTime(),
      UpdatedBy = FindActor(source.UpdatedBy),
      UpdatedOn = source.UpdatedOn.AsUniversalTime()
    };
  }

  private CharacterTalentModel ToCharacterTalent(CharacterTalentEntity source)
  {
    TalentEntity talent = source.Talent ?? throw new ArgumentException("The talent is required.", nameof(source));
    CharacterTalentModel destination = new()
    {
      Id = source.Id,
      Talent = ToTalent(talent),
      Qualifier = source.Qualifier,
      Notes = source.Notes,
      CreatedBy = FindActor(source.CreatedBy),
      CreatedOn = source.CreatedOn.AsUniversalTime(),
      UpdatedBy = FindActor(source.UpdatedBy),
      UpdatedOn = source.UpdatedOn.AsUniversalTime()
    };
    destination.Discounts.AddRange(source.GetDiscounts());
    return destination;
  }

  private static void CalculateAttributes(CharacterEntity source, CharacterModel destination)
  {
    CharacterAttributesEntity attributes = CharacterAttributesEntity.Parse(source.Attributes);

    destination.Attributes.Dexterity.Starting = attributes.Dexterity.Starting;
    destination.Attributes.Health.Starting = attributes.Health.Starting;
    destination.Attributes.Intellect.Starting = attributes.Intellect.Starting;
    destination.Attributes.Senses.Starting = attributes.Senses.Starting;
    destination.Attributes.Vigor.Starting = attributes.Vigor.Starting;

    destination.Attributes.Dexterity.Progression = attributes.Dexterity.Progression;
    destination.Attributes.Health.Progression = attributes.Health.Progression;
    destination.Attributes.Intellect.Progression = attributes.Intellect.Progression;
    destination.Attributes.Senses.Progression = attributes.Senses.Progression;
    destination.Attributes.Vigor.Progression = attributes.Vigor.Progression;

    // TODO(fpion): Bonuses
  }
  private static void CalculateStatistics(CharacterModel character)
  {
    character.Statistics.Dodge.Base = 10 + character.Attributes.Dexterity.Total;
    character.Statistics.Initiative.Base = 2 * character.Attributes.Senses.Total;
    character.Statistics.Learning.Base = Math.Max(
      5 + character.Attributes.Intellect.Total + (character.Level / 5 * (2 + character.Attributes.Intellect.Total)),
      5 + (character.Level / 5));
    character.Statistics.Load.Base = 10 * (5 + character.Attributes.Vigor.Total);
    character.Statistics.Power.Base = 5 + (character.Attributes.Senses.Total * 2);
    character.Statistics.Precision.Base = 5 + (character.Attributes.Dexterity.Total * 2);
    character.Statistics.Stratagem.Base = 5 + (character.Attributes.Intellect.Total * 2);
    character.Statistics.Strength.Base = 5 + (character.Attributes.Vigor.Total * 2);

    int constitution = (25 + character.Level) * (5 + character.Attributes.Health.Total) / 5;
    character.Statistics.Stamina.Base = constitution;
    character.Statistics.Vitality.Base = constitution;

    // TODO(fpion): Bonuses
  }
  private static void CalculateSkills(CharacterEntity source, CharacterModel destination)
  {
    IReadOnlyDictionary<Skill, int> ranks = source.GetSkillRanks();

    Dictionary<Skill, int> talents = new(capacity: 20);
    foreach (CharacterTalentModel acquired in destination.Talents)
    {
      if (acquired.Talent.Skill.HasValue)
      {
        talents[acquired.Talent.Skill.Value] = talents.GetValueOrDefault(acquired.Talent.Skill.Value) + 1;
      }
    }

    destination.Skills.Acrobatics.Rank = ranks.GetValueOrDefault(Skill.Acrobatics);
    destination.Skills.Acrobatics.Talents = talents.GetValueOrDefault(Skill.Acrobatics);
    destination.Skills.Acrobatics.Attribute = destination.Attributes.Dexterity.Total;

    destination.Skills.Athletics.Rank = ranks.GetValueOrDefault(Skill.Athletics);
    destination.Skills.Athletics.Talents = talents.GetValueOrDefault(Skill.Athletics);
    destination.Skills.Athletics.Attribute = destination.Attributes.Vigor.Total;

    destination.Skills.Crafting.Rank = ranks.GetValueOrDefault(Skill.Crafting);
    destination.Skills.Crafting.Talents = talents.GetValueOrDefault(Skill.Crafting);
    destination.Skills.Crafting.Attribute = destination.Attributes.Dexterity.Total;

    destination.Skills.Deception.Rank = ranks.GetValueOrDefault(Skill.Deception);
    destination.Skills.Deception.Talents = talents.GetValueOrDefault(Skill.Deception);

    destination.Skills.Diplomacy.Rank = ranks.GetValueOrDefault(Skill.Diplomacy);
    destination.Skills.Diplomacy.Talents = talents.GetValueOrDefault(Skill.Diplomacy);

    destination.Skills.Discipline.Rank = ranks.GetValueOrDefault(Skill.Discipline);
    destination.Skills.Discipline.Talents = talents.GetValueOrDefault(Skill.Discipline);
    destination.Skills.Discipline.Attribute = destination.Attributes.Health.Total;

    destination.Skills.Insight.Rank = ranks.GetValueOrDefault(Skill.Insight);
    destination.Skills.Insight.Talents = talents.GetValueOrDefault(Skill.Insight);
    destination.Skills.Insight.Attribute = destination.Attributes.Senses.Total;

    destination.Skills.Investigation.Rank = ranks.GetValueOrDefault(Skill.Investigation);
    destination.Skills.Investigation.Talents = talents.GetValueOrDefault(Skill.Investigation);
    destination.Skills.Investigation.Attribute = destination.Attributes.Intellect.Total;

    destination.Skills.Knowledge.Rank = ranks.GetValueOrDefault(Skill.Knowledge);
    destination.Skills.Knowledge.Talents = talents.GetValueOrDefault(Skill.Knowledge);
    destination.Skills.Knowledge.Attribute = destination.Attributes.Intellect.Total;

    destination.Skills.Linguistics.Rank = ranks.GetValueOrDefault(Skill.Linguistics);
    destination.Skills.Linguistics.Talents = talents.GetValueOrDefault(Skill.Linguistics);
    destination.Skills.Linguistics.Attribute = destination.Attributes.Intellect.Total;

    destination.Skills.Medicine.Rank = ranks.GetValueOrDefault(Skill.Medicine);
    destination.Skills.Medicine.Talents = talents.GetValueOrDefault(Skill.Medicine);
    destination.Skills.Medicine.Attribute = destination.Attributes.Intellect.Total;

    destination.Skills.Melee.Rank = ranks.GetValueOrDefault(Skill.Melee);
    destination.Skills.Melee.Talents = talents.GetValueOrDefault(Skill.Melee);
    destination.Skills.Melee.Attribute = destination.Attributes.Vigor.Total;

    destination.Skills.Occultism.Rank = ranks.GetValueOrDefault(Skill.Occultism);
    destination.Skills.Occultism.Talents = talents.GetValueOrDefault(Skill.Occultism);
    destination.Skills.Occultism.Attribute = destination.Attributes.Senses.Total;

    destination.Skills.Perception.Rank = ranks.GetValueOrDefault(Skill.Perception);
    destination.Skills.Perception.Talents = talents.GetValueOrDefault(Skill.Perception);
    destination.Skills.Perception.Attribute = destination.Attributes.Senses.Total;

    destination.Skills.Orientation.Rank = ranks.GetValueOrDefault(Skill.Orientation);
    destination.Skills.Orientation.Talents = talents.GetValueOrDefault(Skill.Orientation);
    destination.Skills.Orientation.Attribute = destination.Attributes.Dexterity.Total;

    destination.Skills.Performance.Rank = ranks.GetValueOrDefault(Skill.Performance);
    destination.Skills.Performance.Talents = talents.GetValueOrDefault(Skill.Performance);

    destination.Skills.Resistance.Rank = ranks.GetValueOrDefault(Skill.Resistance);
    destination.Skills.Resistance.Talents = talents.GetValueOrDefault(Skill.Resistance);
    destination.Skills.Resistance.Attribute = destination.Attributes.Health.Total;

    destination.Skills.Stealth.Rank = ranks.GetValueOrDefault(Skill.Stealth);
    destination.Skills.Stealth.Talents = talents.GetValueOrDefault(Skill.Stealth);
    destination.Skills.Stealth.Attribute = destination.Attributes.Dexterity.Total;

    destination.Skills.Thievery.Rank = ranks.GetValueOrDefault(Skill.Thievery);
    destination.Skills.Thievery.Talents = talents.GetValueOrDefault(Skill.Thievery);
    destination.Skills.Thievery.Attribute = destination.Attributes.Dexterity.Total;

    destination.Skills.Survival.Rank = ranks.GetValueOrDefault(Skill.Survival);
    destination.Skills.Survival.Talents = talents.GetValueOrDefault(Skill.Survival);
    destination.Skills.Survival.Attribute = destination.Attributes.Senses.Total;

    // TODO(fpion): Bonuses
  }
  private static void CalculateSpeeds(CharacterModel character)
  {
    LineageModel lineage = character.Lineage;
    LineageSpeedsModel speeds = lineage.Speeds;

    LineageModel? parent = lineage.Parent;
    if (parent is not null)
    {
      if ((parent.Speeds.Walk ?? 0) > (speeds.Walk ?? 0))
      {
        speeds.Walk = parent.Speeds.Walk;
      }
      if ((parent.Speeds.Climb ?? 0) > (speeds.Climb ?? 0))
      {
        speeds.Climb = parent.Speeds.Climb;
      }
      if ((parent.Speeds.Swim ?? 0) > (speeds.Swim ?? 0))
      {
        speeds.Swim = parent.Speeds.Swim;
      }
      if ((parent.Speeds.Hover && !speeds.Hover) || (parent.Speeds.Hover == speeds.Hover && (parent.Speeds.Fly ?? 0) > (speeds.Fly ?? 0)))
      {
        speeds.Fly = parent.Speeds.Fly;
        speeds.Hover = parent.Speeds.Hover;
      }
      if ((parent.Speeds.Climb ?? 0) > (speeds.Climb ?? 0))
      {
        speeds.Climb = parent.Speeds.Climb;
      }
    }

    character.Speeds.Walk.Lineage = speeds.Walk ?? 0;
    character.Speeds.Climb.Lineage = speeds.Climb ?? 0;
    character.Speeds.Swim.Lineage = speeds.Swim ?? 0;
    character.Speeds.Fly.Lineage = speeds.Fly ?? 0;
    character.Speeds.Hover = speeds.Hover;
    character.Speeds.Burrow.Lineage = speeds.Burrow ?? 0;

    // TODO(fpion): Bonuses

    // TODO(fpion): Encumbrance
  }
  private static void CalculatePoints(CharacterModel character)
  {
    character.Points.Attributes = (int)Math.Floor((character.Level + 5) / 10.0) - character.Attributes.PointsSpent;
    character.Points.Skills = character.Statistics.Learning.Total - character.Skills.PointsSpent;
    character.Points.Talents = 12 + character.Level - character.Talents.Sum(acquisition => acquisition.Cost); // TODO(fpion): Spells
  }

  private static CharacterAppearanceModel GetAppearance(CharacterEntity character) => new()
  {
    Height = character.Height,
    Weight = character.Weight,
    Age = character.Age,
    Skin = character.Skin,
    Eyes = character.Eyes,
    Hair = character.Hair
  };
  private static CharacterPersonalityModel GetPersonality(CharacterEntity character) => new()
  {
    Traits = character.Traits,
    Ideals = character.Ideals,
    Flaws = character.Flaws
  };
}
