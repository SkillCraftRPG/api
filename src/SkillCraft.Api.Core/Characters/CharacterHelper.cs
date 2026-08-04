using Logitar;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Characters;

internal static class CharacterHelper
{
  public static IReadOnlyCollection<CustomizationId> ValidateCustomizations(WorldId worldId, IEnumerable<Customization> customizations, string propertyName)
  {
    int disabilities = 0;
    int gifts = 0;
    foreach (Customization customization in customizations)
    {
      WorldMismatchException.ThrowIfMismatch(worldId, customization.WorldId, propertyName);
      switch (customization.Kind)
      {
        case CustomizationKind.Disability:
          disabilities++;
          break;
        case CustomizationKind.Gift:
          gifts++;
          break;
      }
    }
    if (disabilities != gifts)
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }

    return customizations.Select(customization => customization.Id).Distinct().ToList().AsReadOnly();
  }

  public static IReadOnlyCollection<LanguageId> ValidateLanguages(WorldId worldId, IEnumerable<Language> languages, Lineage lineage, Lineage? parent, string propertyName)
  {
    int extraLanguages = lineage.Languages.Extra;
    HashSet<LanguageId> grantedLanguageIds = lineage.Languages.Ids.ToHashSet();
    if (parent is not null)
    {
      extraLanguages += parent.Languages.Extra;
      grantedLanguageIds.AddRange(parent.Languages.Ids);
    }

    List<Language> grantedLanguages = new(capacity: languages.Count());
    foreach (Language language in languages)
    {
      WorldMismatchException.ThrowIfMismatch(worldId, language.WorldId, propertyName);
      if (grantedLanguageIds.Contains(language.Id))
      {
        grantedLanguages.Add(language);
      }
    }
    if (grantedLanguages.Count > 0)
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }

    IReadOnlyCollection<LanguageId> languageIds = languages.Select(language => language.Id).Distinct().ToList().AsReadOnly();
    if (languageIds.Count != extraLanguages)
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }

    return languageIds;
  }

  public static void ValidateLineage(WorldId worldId, Lineage lineage, Lineage? parent, string propertyName)
  {
    WorldMismatchException.ThrowIfMismatch(worldId, lineage.WorldId, propertyName);

    if (lineage.ParentId.HasValue)
    {
      if (parent is null)
      {
        throw new ArgumentNullException(nameof(parent));
      }
      else if (parent.Id != lineage.ParentId.Value)
      {
        throw new ArgumentException($"The lineage parent 'Id={parent.Id}' was not expected ({lineage.ParentId}).", nameof(parent));
      }
    }
    else if (parent is not null)
    {
      throw new ArgumentException("A parent lineage was not expected.", nameof(parent));
    }
  }
}
