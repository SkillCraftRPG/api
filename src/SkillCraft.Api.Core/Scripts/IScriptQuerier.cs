using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Scripts;

namespace SkillCraft.Api.Core.Scripts;

public interface IScriptQuerier
{
  Task<ScriptModel> ReadAsync(Script script, CancellationToken cancellationToken = default);
  Task<ScriptModel?> ReadAsync(ScriptId id, CancellationToken cancellationToken = default);
  Task<ScriptModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ScriptModel>> SearchAsync(SearchScriptsPayload payload, CancellationToken cancellationToken = default);
}
