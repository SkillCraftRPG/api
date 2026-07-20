using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Languages.Models;

public record SearchLanguagesPayload : SearchPayload
{
  public Guid? ScriptId { get; set; }

  public new List<LanguageSortOption> Sort { get; set; } = [];
}
