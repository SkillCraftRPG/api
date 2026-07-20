using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Core.Scripts.Models;

public record SearchScriptsPayload : SearchPayload
{
  public new List<ScriptSortOption> Sort { get; set; } = [];
}
