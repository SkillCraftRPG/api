using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Contracts.Customizations;
using SkillCraft.Api.Contracts.Educations;
using SkillCraft.Api.Contracts.Languages;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

internal class GameMapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public GameMapper()
  {
  }

  public GameMapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public CasteModel ToCaste(CasteEntity source)
  {
    CasteModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      Description = source.Description,
      Skill = source.Skill,
      WealthRoll = source.WealthRoll
    };

    if (source.FeatureName is not null)
    {
      destination.Feature = new FeatureModel(source.FeatureName, source.FeatureDescription);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public CustomizationModel ToCustomization(CustomizationEntity source)
  {
    CustomizationModel destination = new()
    {
      Id = source.Id,
      Kind = source.Kind,
      Name = source.Name,
      Summary = source.Summary,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public EducationModel ToEducation(EducationEntity source)
  {
    EducationModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      Description = source.Description,
      Skill = source.Skill,
      WealthMultiplier = source.WealthMultiplier
    };

    if (source.FeatureName is not null)
    {
      destination.Feature = new FeatureModel(source.FeatureName, source.FeatureDescription);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public LineageModel ToLineage(LineageEntity source)
  {
    LineageModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      Description = source.Description,
      Speeds = new SpeedsModel(source.Walk, source.Climb, source.Swim, source.Fly, source.Hover, source.Burrow),
      Size = new SizeModel(source.SizeCategory, source.Height),
      Weight = new WeightModel(source.Malnutrition, source.Skinny, source.Normal, source.Overweight, source.Obese),
      Age = new AgeModel(source.Teenager, source.Adult, source.Mature, source.Venerable)
    };

    if (source.Parent is not null)
    {
      destination.Parent = ToLineage(source.Parent);
    }
    else if (source.ParentId.HasValue)
    {
      throw new ArgumentException("The parent is required.", nameof(source));
    }

    foreach (LineageLanguageEntity entity in source.Languages)
    {
      LanguageEntity language = entity.Language ?? throw new ArgumentException("The language is required.", nameof(source));
      destination.Languages.Items.Add(ToLanguage(language));
    }
    destination.Languages.Extra = source.ExtraLanguages;
    destination.Languages.Text = source.LanguagesText;

    MapAggregate(source, destination);

    return destination;
  }

  public PartyModel ToParty(PartyEntity source)
  {
    PartyModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public ScriptModel ToScript(ScriptEntity source)
  {
    ScriptModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public TalentModel ToTalent(TalentEntity source)
  {
    TalentModel destination = new()
    {
      Id = source.Id,
      Tier = source.Tier,
      Name = source.Name,
      Summary = source.Summary,
      Description = source.Description,
      AllowMultiplePurchases = source.AllowMultiplePurchases,
      Skill = source.Skill
    };

    if (source.RequiredTalent is not null)
    {
      destination.RequiredTalent = ToTalent(source.RequiredTalent);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public LanguageModel ToLanguage(LanguageEntity source)
  {
    LanguageModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      Description = source.Description,
      TypicalSpeakers = source.TypicalSpeakers
    };

    if (source.Script is not null)
    {
      destination.Script = ToScript(source.Script);
    }
    else if (source.ScriptId.HasValue)
    {
      throw new ArgumentException("The script is required.", nameof(source));
    }

    MapAggregate(source, destination);

    return destination;
  }

  public WorldModel ToWorld(WorldEntity source)
  {
    WorldModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Description = source.Description,
      Owner = FindActor(source.OwnerId)
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Version = source.Version;

    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(string? id) => (string.IsNullOrWhiteSpace(id) ? null : TryFindActor(id)) ?? _system;
  private Actor FindActor(ActorId? id) => (id.HasValue ? TryFindActor(id.Value) : null) ?? _system;
  private Actor? TryFindActor(string? id) => string.IsNullOrWhiteSpace(id) ? null : TryFindActor(new ActorId(id));
  private Actor? TryFindActor(ActorId? id) => id.HasValue && _actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
}
