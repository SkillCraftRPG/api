using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts.Scripts;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Scripts;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class ScriptQuerier : IScriptQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<ScriptEntity> _scripts;
  private readonly ISqlHelper _sqlHelper;

  public ScriptQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _scripts = game.Scripts;
    _sqlHelper = sqlHelper;
  }

  public async Task<ScriptModel> ReadAsync(Script script, CancellationToken cancellationToken)
  {
    return await ReadAsync(script.Id, cancellationToken) ?? throw new InvalidOperationException($"The script entity 'StreamId={script.Id}' was not found.");
  }
  public async Task<ScriptModel?> ReadAsync(ScriptId id, CancellationToken cancellationToken)
  {
    ScriptEntity? script = await _scripts.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return script is null ? null : await MapAsync(script, cancellationToken);
  }
  public async Task<ScriptModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ScriptEntity? script = await _scripts.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.Id == id)
      .SingleOrDefaultAsync(cancellationToken);
    return script is null ? null : await MapAsync(script, cancellationToken);
  }

  public async Task<SearchResults<ScriptModel>> SearchAsync(SearchScriptsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Scripts.Table).SelectAll(GameDb.Scripts.Table)
      .ApplyIdFilter(GameDb.Scripts.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Scripts.Name, GameDb.Scripts.Summary);

    IQueryable<ScriptEntity> query = _scripts.FromQuery(builder).AsNoTracking()
      .WhereWorld(_context.WorldId);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ScriptEntity>? ordered = null;
    foreach (ScriptSortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        ScriptSort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        ScriptSort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        ScriptSort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The script sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ScriptEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ScriptModel> scripts = await MapAsync(entities, cancellationToken);

    return new SearchResults<ScriptModel>(scripts, total);
  }

  private async Task<ScriptModel> MapAsync(ScriptEntity script, CancellationToken cancellationToken)
  {
    return (await MapAsync([script], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ScriptModel>> MapAsync(IEnumerable<ScriptEntity> scripts, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = scripts.SelectMany(script => script.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return scripts.Select(mapper.ToScript).ToList().AsReadOnly();
  }
}
