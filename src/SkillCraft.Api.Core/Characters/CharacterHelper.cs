using Logitar;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Customizations;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Core.Languages;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Worlds;

namespace SkillCraft.Api.Core.Characters;

internal static class CharacterHelper
{
  private const int MinimumSkillsTrained = 6;
  private const int MinimumSpentPoints = 10;
  private const int MaximumSpentPoints = 12;

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

  public static IReadOnlyDictionary<Guid, CharacterTalent> ValidateTalents(
    WorldId worldId,
    IEnumerable<CharacterTalent> talents,
    Lineage lineage,
    Lineage? parent,
    Caste caste,
    Education education,
    IEnumerable<Customization> customizations,
    string propertyName)
  {
    int capacity = talents.Count();

    HashSet<Guid> lineageIds = new(capacity: 2);
    lineageIds.Add(lineage.ResourceId);
    if (parent is not null)
    {
      lineageIds.Add(parent.ResourceId);
    }

    HashSet<Guid> customizationIds = customizations.Select(customization => customization.ResourceId).ToHashSet();

    HashSet<TalentId> talentIds = talents.Select(acquired => acquired.TalentId).ToHashSet();

    int spentPoints = 0;
    Dictionary<TalentId, int> duplicates = new(capacity);
    HashSet<Skill> skills = new(capacity);
    foreach (CharacterTalent acquired in talents)
    {
      Talent talent = acquired.Talent ?? throw new ArgumentException("The talent is requred.", nameof(talents));
      WorldMismatchException.ThrowIfMismatch(worldId, acquired.Talent.WorldId, propertyName);

      if (talent.Tier.Value > 0)
      {
        throw new NotImplementedException(); // TODO(fpion): DomainException
      }
      if (talent.RequiredTalentId.HasValue && !talentIds.Contains(talent.RequiredTalentId.Value))
      {
        throw new NotImplementedException(); // TODO(fpion): DomainException
      }

      spentPoints += acquired.Cost;

      if (!talent.AllowMultiplePurchases)
      {
        if (duplicates.TryGetValue(talent.Id, out int count))
        {
          duplicates[talent.Id] = count + 1;
        }
        else
        {
          duplicates[talent.Id] = 1;
        }
      }

      if (talent.Skill.HasValue)
      {
        skills.Add(talent.Skill.Value);
      }

      foreach (CharacterTalentDiscount discount in acquired.Discounts)
      {
        switch (discount.Source)
        {
          case CharacterTalentDiscountSource.Customization:
            if (!customizationIds.Contains(Guid.Parse(discount.Target)))
            {
              throw new NotImplementedException(); // TODO(fpion): DomainException
            }
            break;
          case CharacterTalentDiscountSource.Lineage:
            if (!lineageIds.Contains(Guid.Parse(discount.Target)))
            {
              throw new NotImplementedException(); // TODO(fpion): DomainException
            }
            break;
        }
      }
    }

    IEnumerable<Guid> duplicateIds = duplicates.Where(x => x.Value > 1).Select(x => x.Key.ResourceId);
    if (duplicateIds.Any())
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }

    if (skills.Count < MinimumSkillsTrained)
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }
    if (caste.Skill.HasValue && !skills.Contains(caste.Skill.Value))
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }
    if (education.Skill.HasValue && !skills.Contains(education.Skill.Value))
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }

    if (spentPoints < MinimumSpentPoints)
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }
    else if (spentPoints > MaximumSpentPoints)
    {
      throw new NotImplementedException(); // TODO(fpion): DomainException
    }

    return talents.ToDictionary(x => Guid.NewGuid(), x => x).AsReadOnly();
  }
}
