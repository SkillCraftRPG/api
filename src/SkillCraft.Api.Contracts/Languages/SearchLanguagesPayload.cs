using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Languages;

public record SearchLanguagesPayload : SearchPayload
{
  public new List<LanguageSortOption> Sort { get; set; } = [];
}
