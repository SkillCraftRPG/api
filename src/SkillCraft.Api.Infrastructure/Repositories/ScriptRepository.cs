using Logitar.EventSourcing;
using SkillCraft.Api.Core.Scripts;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class ScriptRepository : Repository, IScriptRepository
{
  public ScriptRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Script?> LoadAsync(ScriptId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Script>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Script>> LoadAsync(IEnumerable<ScriptId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Script>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Script script, CancellationToken cancellationToken)
  {
    await base.SaveAsync(script, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Script> scripts, CancellationToken cancellationToken)
  {
    await base.SaveAsync(scripts, cancellationToken);
  }
}
