using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure;

internal class MapperOld // TODO(fpion): remove this
{
  private readonly Dictionary<Guid, Actor> _actors = [];
  private readonly Actor _system = new();

  public MapperOld()
  {
  }

  public MapperOld(IEnumerable<KeyValuePair<Guid, Actor>> actors)
  {
    foreach (KeyValuePair<Guid, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public LineageModel ToLineage(
    Lineage source,
    IReadOnlyDictionary<int, ScriptModel>? scripts = null,
    IReadOnlyList<LanguageEntity>? languages = null,
    IReadOnlyList<LanguageEntity>? parentLanguages = null,
    Mapper? languageMapper = null)
  {
    LanguageModel MapLanguage(LanguageEntity language)
    {
      if (languageMapper is not null)
      {
        LanguageModel model = languageMapper.ToLanguage(language);
        if (language.ScriptId is int key && scripts is not null && scripts.TryGetValue(key, out ScriptModel? script))
        {
          model.Script = script;
        }
        return model;
      }

      ScriptModel? scriptModel = null;
      if (language.ScriptId is int scriptKey && scripts is not null)
      {
        scripts.TryGetValue(scriptKey, out scriptModel);
      }
      return new LanguageModel
      {
        Id = language.Id,
        Name = language.Name,
        Summary = language.Summary,
        Content = language.Content,
        TypicalSpeakers = language.TypicalSpeakers,
        Script = scriptModel,
        Version = language.Version
      };
    }

    LineageModel destination = new()
    {
      Id = source.Id,
      Name = source.Name,
      Summary = source.Summary,
      Content = source.Content
    };

    if (source.Parent is not null)
    {
      destination.Parent = ToLineage(source.Parent, scripts, parentLanguages, null, languageMapper);
    }

    foreach (LineageFeature feature in source.Features)
    {
      destination.Features.Add(ToLineageFeature(feature));
    }

    foreach (LanguageEntity language in languages ?? [])
    {
      destination.Languages.Granted.Add(MapLanguage(language));
    }
    destination.Languages.Extra = source.ExtraLanguages;
    destination.Languages.Content = source.LanguagesContent;

    destination.Names.Family.AddRange(LineageNames.Decode(source.FamilyNames));
    destination.Names.Female.AddRange(LineageNames.Decode(source.FemaleNames));
    destination.Names.Male.AddRange(LineageNames.Decode(source.MaleNames));
    destination.Names.Unisex.AddRange(LineageNames.Decode(source.UnisexNames));
    destination.Names.Custom.AddRange(LineageNames.DecodeCustom(source.CustomNames).Select(category => new NameCategory(category.Key, category.Value)));
    destination.Names.Content = source.NamesContent;

    destination.Speeds.Walk = source.Walk;
    destination.Speeds.Climb = source.Climb;
    destination.Speeds.Swim = source.Swim;
    destination.Speeds.Fly = source.Fly;
    destination.Speeds.Hover = source.Hover;
    destination.Speeds.Burrow = source.Burrow;

    destination.Size.Category = source.SizeCategory;
    destination.Size.Height = source.HeightRoll;

    destination.Weight.Malnutrition = source.Malnutrition;
    destination.Weight.Skinny = source.Skinny;
    destination.Weight.Normal = source.NormalWeight;
    destination.Weight.Overweight = source.Overweight;
    destination.Weight.Obese = source.Obese;

    destination.Age.Teenager = source.Teenager;
    destination.Age.Adult = source.Adult;
    destination.Age.Mature = source.Mature;
    destination.Age.Venerable = source.Venerable;

    MapAggregate(source, destination);

    return destination;
  }

  public LineageFeatureModel ToLineageFeature(LineageFeature feature) => new()
  {
    Id = feature.Id,
    Name = feature.Name,
    Content = feature.Content,
    CreatedBy = FindActor(feature.CreatedBy),
    CreatedOn = feature.CreatedOn.AsUniversalTime(),
    UpdatedBy = FindActor(feature.UpdatedBy),
    UpdatedOn = feature.UpdatedOn.AsUniversalTime()
  };

  public TalentModel ToTalent(TalentEntity source)
  {
    TalentModel destination = new()
    {
      Id = source.Id,
      Tier = source.Tier,
      Name = source.Name,
      Summary = source.Summary,
      Content = source.Content,
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

  private void MapAggregate(object source, Aggregate destination)
  {
    if (source is IAuditable auditable)
    {
      destination.CreatedBy = FindActor(auditable.CreatedBy);
      destination.CreatedOn = auditable.CreatedOn.AsUniversalTime();
      destination.UpdatedBy = FindActor(auditable.UpdatedBy);
      destination.UpdatedOn = auditable.UpdatedOn.AsUniversalTime();
    }

    if (source is IVersioned versioned)
    {
      destination.Version = versioned.Version;
    }
  }

  private Actor FindActor(Guid id) => _actors.TryGetValue(id, out Actor? actor) ? actor : _system;
}
