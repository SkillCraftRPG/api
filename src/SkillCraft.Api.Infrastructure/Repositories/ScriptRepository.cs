using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Scripts.Events;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class ScriptRepository : Repository, IScriptRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public ScriptRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Script[] scripts)
  {
    foreach (Script script in scripts)
    {
      Database.Scripts.Add(script);
      base.RecordChange(new ScriptCreated(script));
    }
  }
  public void Remove(Script script)
  {
    Database.Scripts.Remove(script);
    base.RecordChange(new ScriptDeleted(script, _context.UserId));
  }
  public void Update(Script script, ScriptUpdated record)
  {
    Database.Scripts.Update(script);
    base.RecordChange(record);
  }

  public async Task<Script?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Scripts.SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<ScriptModel> ReadAsync(Script script, CancellationToken cancellationToken)
  {
    return await ReadAsync(script.Id, cancellationToken) ?? throw new InvalidOperationException($"The script 'Id={script.Id}' was not found.");
  }
  public async Task<ScriptModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Script? script = await Database.Scripts.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .SingleOrDefaultAsync(cancellationToken);

    return script is null ? null : await MapAsync(script, cancellationToken);
  }

  public virtual async Task<SearchResults<ScriptModel>> SearchAsync(SearchScriptsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Scripts.Table).SelectAll(Db.Scripts.Table)
      .Where(Db.Scripts.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Scripts.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Scripts.Name, Db.Scripts.Summary);

    IQueryable<Script> query = Database.Scripts.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Script>? ordered = null;
    foreach (ScriptSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ScriptSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ScriptSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case ScriptSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Script[] scripts = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ScriptModel> items = await MapAsync(scripts, cancellationToken);

    return new SearchResults<ScriptModel>(items, total);
  }

  private async Task<ScriptModel> MapAsync(Script script, CancellationToken cancellationToken)
  {
    return (await MapAsync([script], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ScriptModel>> MapAsync(IEnumerable<Script> scripts, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = scripts.SelectMany(script => script.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return scripts.Select(mapper.ToScript).ToList().AsReadOnly();
  }
}
