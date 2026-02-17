using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Lineages.Commands;

internal abstract class SaveLineage
{
  protected virtual ILanguageRepository LanguageRepository { get; }

  protected SaveLineage(ILanguageRepository languageRepository)
  {
    LanguageRepository = languageRepository;
  }

  protected virtual async Task SetLanguagesAsync(Lineage lineage, LanguagesPayload payload, WorldId worldId, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Language> languages = [];
    if (payload.Ids.Count > 0)
    {
      HashSet<LanguageId> languageIds = payload.Ids.Select(entityId => new LanguageId(entityId, worldId)).ToHashSet();
      languages = await LanguageRepository.LoadAsync(languageIds, cancellationToken);

      HashSet<LanguageId> missingIds = languageIds.Except(languages.Select(language => language.Id)).ToHashSet();
      if (missingIds.Count > 0)
      {
        throw new LanguagesNotFoundException(worldId, missingIds, propertyName: "Languages.Ids");
      }
    }
    lineage.Languages = new LineageLanguages(languages, payload.Extra, Description.TryCreate(payload.Text));
  }

  protected virtual void SetNames(Lineage lineage, NamesModel names)
  {
    Dictionary<string, IReadOnlyCollection<string>> custom = new(capacity: names.Custom.Count);
    foreach (NameCategory nameCategory in names.Custom)
    {
      custom[nameCategory.Category] = nameCategory.Names;
    }
    lineage.Names = new Names(names.Family, names.Female, names.Male, names.Unisex, custom, Description.TryCreate(names.Text));
  }
}
