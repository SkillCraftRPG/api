using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Core.Scripts.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class ScriptQuerier : IScriptQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<ScriptEntity> _scripts;
  private readonly ISqlHelper _sqlHelper;

  public ScriptQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _scripts = database.Scripts;
    _sqlHelper = sqlHelper;
  }

  public async Task<int?> FindKeyAsync(ScriptId id, CancellationToken cancellationToken)
  {
    return await _scripts.AsNoTracking()
      .Where(x => x.StreamId == id.Value)
      .Select(x => (int?)x.ScriptId)
      .SingleOrDefaultAsync(cancellationToken);
  }
  public async Task<int?> FindKeyAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _scripts.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldUid)
      .Select(x => (int?)x.ScriptId)
      .SingleOrDefaultAsync(cancellationToken);
  }

  public async Task<ScriptModel> ReadAsync(Script script, CancellationToken cancellationToken)
  {
    return await ReadAsync(script.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The script entity 'StreamId={script.Id}' was not found.");
  }
  public async Task<ScriptModel?> ReadAsync(ScriptId id, CancellationToken cancellationToken)
  {
    ScriptEntity? script = await _scripts.AsNoTracking()
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);

    return script is null ? null : await MapAsync(script, cancellationToken);
  }
  public async Task<ScriptModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ScriptEntity? script = await _scripts.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldUid)
      .SingleOrDefaultAsync(cancellationToken);

    return script is null ? null : await MapAsync(script, cancellationToken);
  }

  public virtual async Task<SearchResults<ScriptModel>> SearchAsync(SearchScriptsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Scripts.Table).SelectAll(Db.Scripts.Table)
      .Where(Db.Scripts.WorldId, Operators.IsEqualTo(_context.WorldUid))
      .ApplyIdFilter(Db.Scripts.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Scripts.Name, Db.Scripts.Summary);

    IQueryable<ScriptEntity> query = _scripts.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ScriptEntity>? ordered = null;
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

    ScriptEntity[] scripts = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ScriptModel> items = await MapAsync(scripts, cancellationToken);

    return new SearchResults<ScriptModel>(items, total);
  }

  private async Task<ScriptModel> MapAsync(ScriptEntity script, CancellationToken cancellationToken)
  {
    return (await MapAsync([script], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ScriptModel>> MapAsync(IEnumerable<ScriptEntity> scripts, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = scripts.SelectMany(script => script.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return scripts.Select(mapper.ToScript).ToList().AsReadOnly();
  }
}
