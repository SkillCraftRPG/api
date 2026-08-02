using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class ScriptRepository : Logitar.EventSourcing.Repository, IScriptRepository
{
  private readonly GameContext _database;

  public ScriptRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Script?> LoadAsync(ScriptId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Script>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Script>> LoadAsync(IEnumerable<ScriptId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Script>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Script script, CancellationToken cancellationToken)
  {
    await base.SaveAsync(script, cancellationToken);

    await SynchronizeAsync(script, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Script> scripts, CancellationToken cancellationToken)
  {
    await base.SaveAsync(scripts, cancellationToken);

    foreach (Script script in scripts)
    {
      await SynchronizeAsync(script, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Script script, CancellationToken cancellationToken)
  {
    ScriptEntity? entity = await _database.Scripts.SingleOrDefaultAsync(x => x.StreamId == script.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new ScriptEntity(script);
      _database.Scripts.Add(entity);
    }
    else
    {
      entity.Update(script);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
