using Krakenar.Contracts;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Languages.Models;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Infrastructure.Compendium.Models;

namespace SkillCraft.Api.Infrastructure.Compendium;

internal static class CompendiumMapper
{
  public static CasteModel ToCaste(CasteEntry source)
  {
    CasteModel destination = new()
    {
      Name = source.Name,
      Summary = source.Summary,
      Content = source.HtmlContent,
      Skill = source.Skill?.Value,
      WealthRoll = source.WealthRoll
    };

    if (source.Feature is not null)
    {
      destination.Feature = ToFeature(source.Feature);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public static CustomizationModel ToCustomization(CustomizationEntry source)
  {
    CustomizationModel destination = new()
    {
      Kind = source.Kind,
      Name = source.Name,
      Summary = source.Summary,
      Content = source.HtmlContent
    };

    MapAggregate(source, destination);

    return destination;
  }

  public static EducationModel ToEducation(EducationEntry source)
  {
    EducationModel destination = new()
    {
      Name = source.Name,
      Summary = source.Summary,
      Content = source.HtmlContent,
      Skill = source.Skill?.Value,
      WealthMultiplier = source.WealthMultiplier
    };

    if (source.Feature is not null)
    {
      destination.Feature = ToFeature(source.Feature);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public static FeatureModel ToFeature(FeatureEntry source) => new(source.Name, source.HtmlContent);

  public static LanguageModel ToLanguage(LanguageEntry source)
  {
    LanguageModel destination = new()
    {
      Name = source.Name,
      Summary = source.Summary,
      Content = source.HtmlContent,
      TypicalSpeakers = source.TypicalSpeakers
    };

    if (source.Script is not null)
    {
      destination.Script = ToScript(source.Script);
    }

    MapAggregate(source, destination);

    return destination;
  }

  public static ScriptModel ToScript(ScriptEntry source)
  {
    ScriptModel destination = new()
    {
      Name = source.Name,
      Summary = source.Summary,
      Content = source.HtmlContent
    };

    MapAggregate(source, destination);

    return destination;
  }

  public static LineageModel ToSpecies(SpeciesEntry source)
  {
    LineageModel destination = ToLineage(source);

    destination.Size = new LineageSizeModel
    {
      Category = source.Size.Category,
      Height = source.Size.Roll
    };
    destination.Weight = new LineageWeightModel
    {
      Malnutrition = source.Weight.Malnutrition,
      Skinny = source.Weight.Skinny,
      Normal = source.Weight.Normal,
      Overweight = source.Weight.Overweight,
      Obese = source.Weight.Obese
    };
    destination.Age = ToAge(source.Age);

    foreach (EthnicityEntry ethnicity in source.Ethnicities)
    {
      LineageModel child = ToLineage(ethnicity);
      child.Parent = new LineageModel { Id = destination.Id, Name = destination.Name };
      MapAggregate(destination, child.Parent);
      destination.Children.Add(child);
    }

    return destination;
  }

  public static TalentModel ToTalent(TalentEntry source)
  {
    TalentModel destination = new()
    {
      Tier = source.Tier,
      Name = source.Name,
      Summary = source.Summary,
      Content = source.HtmlContent,
      AllowMultiplePurchases = source.AllowMultiplePurchases,
      Skill = source.Skill?.Value
    };

    if (source.RequiredTalent is not null)
    {
      destination.RequiredTalent = ToTalent(source.RequiredTalent);
    }

    MapAggregate(source, destination);

    return destination;
  }

  private static LineageModel ToLineage(SpeciesEntry source)
  {
    LineageModel destination = new()
    {
      Name = source.Name,
      Summary = source.Summary,
      Content = source.HtmlContent,
      Languages = ToLanguages(source.Languages),
      Names = ToNames(source.Names),
      Speeds = ToSpeeds(source.Speeds),
      Features = source.Features.Select(ToFeature).ToList()
    };

    MapAggregate(source, destination);

    return destination;
  }

  private static LineageModel ToLineage(EthnicityEntry source)
  {
    LineageModel destination = new()
    {
      Name = source.Name,
      Summary = source.Summary,
      Content = source.HtmlContent,
      Languages = ToLanguages(source.Languages),
      Names = ToNames(source.Names),
      Speeds = ToSpeeds(source.Speeds),
      Features = source.Features.Select(ToFeature).ToList()
    };

    MapAggregate(source, destination);

    return destination;
  }

  private static LineageLanguagesModel ToLanguages(SpeciesLanguagesEntry source) => new()
  {
    Granted = source.Items.Select(ToLanguage).ToList(),
    Extra = source.Extra,
    Content = source.Text
  };

  private static LineageNamesModel ToNames(SpeciesNamesEntry source) => new()
  {
    Family = [.. source.Family],
    Female = [.. source.Female],
    Male = [.. source.Male],
    Unisex = [.. source.Unisex],
    Custom = source.Custom.Select(category => new NameCategory(category.Category, category.Values)).ToList(),
    Content = source.Text
  };

  private static LineageSpeedsModel ToSpeeds(SpeciesSpeedsEntry source) => new()
  {
    Walk = source.Walk < 1 ? null : source.Walk,
    Climb = source.Climb < 1 ? null : source.Climb,
    Swim = source.Swim < 1 ? null : source.Swim,
    Fly = source.Fly < 1 ? null : source.Fly,
    Hover = source.Hover,
    Burrow = source.Burrow < 1 ? null : source.Burrow
  };

  private static LineageAgeModel ToAge(SpeciesAgeEntry source)
  {
    if (source.Teenager <= 0 && source.Adult <= 0 && source.Mature <= 0 && source.Venerable <= 0)
    {
      return new();
    }

    return new(source.Teenager, source.Adult, source.Mature, source.Venerable);
  }

  private static void MapAggregate(Aggregate source, Aggregate destination)
  {
    destination.Id = source.Id;
    destination.Version = source.Version;
    destination.CreatedBy = source.CreatedBy;
    destination.CreatedOn = source.CreatedOn;
    destination.UpdatedBy = source.UpdatedBy;
    destination.UpdatedOn = source.UpdatedOn;
  }
}
