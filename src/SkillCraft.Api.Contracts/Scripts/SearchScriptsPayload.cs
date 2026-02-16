using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Scripts;

public record SearchScriptsPayload : SearchPayload
{
  public new List<ScriptSortOption> Sort { get; set; } = [];
}
