using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Scripts;

public interface IScriptService
{
  Task<CreateOrReplaceScriptResult> CreateOrReplaceAsync(CreateOrReplaceScriptPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<ScriptModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ScriptModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<ScriptModel>> SearchAsync(SearchScriptsPayload payload, CancellationToken cancellationToken = default);
  Task<ScriptModel?> UpdateAsync(Guid id, UpdateScriptPayload payload, CancellationToken cancellationToken = default);
}
