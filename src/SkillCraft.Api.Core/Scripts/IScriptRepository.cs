namespace SkillCraft.Api.Core.Scripts;

public interface IScriptRepository
{
  Task<Script?> LoadAsync(ScriptId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Script>> LoadAsync(IEnumerable<ScriptId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Script script, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Script> scripts, CancellationToken cancellationToken = default);
}
