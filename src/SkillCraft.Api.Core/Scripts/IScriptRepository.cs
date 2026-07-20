using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Scripts.Events;
using SkillCraft.Api.Core.Scripts.Models;

namespace SkillCraft.Api.Core.Scripts;

public interface IScriptRepository
{
  void Add(params Script[] scripts);
  void Remove(Script script);
  void Update(Script script, ScriptUpdated record);

  Task<Script?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<ScriptModel> ReadAsync(Script script, CancellationToken cancellationToken = default);
  Task<ScriptModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ScriptModel>> SearchAsync(SearchScriptsPayload payload, CancellationToken cancellationToken = default);
}
