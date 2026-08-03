using Krakenar.Contracts;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Core.Customizations.Models;
using SkillCraft.Api.Core.Educations.Models;
using SkillCraft.Api.Core.Features;
using SkillCraft.Api.Core.Languages.Models;
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
