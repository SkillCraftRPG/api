using Logitar;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Models;

namespace SkillCraft.Api.Core.Lineages;

public static class LineageHelper
{
  public static void SetAge(Lineage lineage, LineageAgeModel age)
  {
    lineage.Teenager = age.Teenager;
    lineage.Adult = age.Adult;
    lineage.Mature = age.Mature;
    lineage.Venerable = age.Venerable;
  }

  public static void SetLanguages(Lineage lineage, IEnumerable<Language> languages, LineageLanguagesPayload payload)
  {
    lineage.Languages.Clear();
    lineage.Languages.AddRange(languages);
    lineage.ExtraLanguages = payload.Extra;
    lineage.LanguagesHtmlContent = payload.HtmlContent?.CleanTrim();
  }

  public static void SetNames(Lineage lineage, LineageNamesModel names)
  {
    lineage.FamilyNames = EncodeNames(names.Family);
    lineage.FemaleNames = EncodeNames(names.Female);
    lineage.MaleNames = EncodeNames(names.Male);
    lineage.UnisexNames = EncodeNames(names.Unisex);
    lineage.CustomNames = EncodeNames(names.Custom);
    lineage.NamesHtmlContent = names.HtmlContent?.CleanTrim();
  }

  public static void SetSize(Lineage lineage, LineageSizeModel size)
  {
    lineage.SizeCategory = size.Category;
    lineage.HeightRoll = size.Height;
  }

  public static void SetSpeeds(Lineage lineage, LineageSpeedsModel speeds)
  {
    lineage.Walk = speeds.Walk;
    lineage.Climb = speeds.Climb;
    lineage.Swim = speeds.Swim;
    lineage.Fly = speeds.Fly;
    lineage.Hover = speeds.Hover;
    lineage.Burrow = speeds.Burrow;
  }

  public static void SetWeight(Lineage lineage, LineageWeightModel weight)
  {
    lineage.Malnutrition = weight.Malnutrition;
    lineage.Skinny = weight.Skinny;
    lineage.NormalWeight = weight.Normal;
    lineage.Overweight = weight.Overweight;
    lineage.Obese = weight.Obese;
  }

  private static string? EncodeNames(IEnumerable<string> names)
  {
    IReadOnlyCollection<string> cleaned = CleanNames(names);
    return cleaned.Count < 1 ? null : JsonSerializer.Serialize(cleaned);
  }

  private static string? EncodeNames(IEnumerable<NameCategory> categories)
  {
    int capacity = categories.Count();
    if (capacity < 1)
    {
      return null;
    }

    Dictionary<string, IReadOnlyCollection<string>> customNames = new(capacity);
    foreach (NameCategory custom in categories)
    {
      string category = custom.Category.Trim();
      IReadOnlyCollection<string> names = CleanNames(custom.Values);
      if (!string.IsNullOrEmpty(category) && names.Count > 0)
      {
        customNames[category] = names;
      }
    }
    return customNames.Count < 1 ? null : JsonSerializer.Serialize(customNames);
  }

  private static IReadOnlyCollection<string> CleanNames(IEnumerable<string> names) => names
    .Where(name => !string.IsNullOrWhiteSpace(name))
    .Select(name => name.Trim())
    .OrderBy(name => name)
    .Distinct()
    .ToList()
    .AsReadOnly();
}
