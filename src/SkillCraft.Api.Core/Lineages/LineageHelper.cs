using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages;

internal static class LineageHelper
{
  public static async Task<LineageLanguages> GetLanguagesAsync(
    ILanguageRepository languageRepository,
    WorldId worldId,
    LineageLanguagesPayload payload,
    CancellationToken cancellationToken)
  {
    HashSet<LanguageId> languageIds = payload.Ids.Select(id => new LanguageId(worldId, id)).ToHashSet();
    IReadOnlyCollection<Language> languages = await languageRepository.LoadAsync(languageIds, cancellationToken);

    HashSet<LanguageId> foundIds = languages.Select(language => language.Id).ToHashSet();
    IEnumerable<LanguageId> missingIds = languageIds.Except(foundIds);
    if (missingIds.Any())
    {
      string propertyName = string.Join('.', nameof(CreateOrReplaceLineagePayload.Languages), nameof(payload.Ids));
      throw new LanguagesNotFoundException(missingIds, propertyName);
    }

    return new LineageLanguages(foundIds, payload.Extra, Content.TryCreate(payload.Content));
  }

  public static LineageNames GetNames(LineageNamesModel payload)
  {
    Dictionary<string, IReadOnlyCollection<string>> custom = new(capacity: payload.Custom.Count);
    foreach (NameCategory category in payload.Custom)
    {
      custom[category.Category] = category.Values;
    }
    return new LineageNames(payload.Family, payload.Female, payload.Male, payload.Unisex, custom, Content.TryCreate(payload.Content));
  }

  public static LineageSize GetSize(LineageSizeModel payload) => new(payload.Category, Roll.TryCreate(payload.Height));

  public static LineageWeight GetWeight(LineageWeightModel payload) => new(
    Roll.TryCreate(payload.Malnutrition),
    Roll.TryCreate(payload.Skinny),
    Roll.TryCreate(payload.Normal),
    Roll.TryCreate(payload.Overweight),
    Roll.TryCreate(payload.Obese));
}
