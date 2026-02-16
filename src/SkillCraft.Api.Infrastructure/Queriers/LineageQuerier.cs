using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts.Lineages;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class LineageQuerier : ILineageQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<LineageEntity> _lineages;
  private readonly ISqlHelper _sqlHelper;

  public LineageQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _lineages = game.Lineages;
    _sqlHelper = sqlHelper;
  }

  public async Task<LineageModel> ReadAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    return await ReadAsync(lineage.Id, cancellationToken) ?? throw new InvalidOperationException($"The lineage entity 'StreamId={lineage.Id}' was not found.");
  }
  public async Task<LineageModel?> ReadAsync(LineageId id, CancellationToken cancellationToken)
  {
    LineageEntity? lineage = await _lineages.AsNoTracking()
      .Include(x => x.Parent)
      .WhereWorld(_context.WorldId)
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return lineage is null ? null : await MapAsync(lineage, cancellationToken);
  }
  public async Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LineageEntity? lineage = await _lineages.AsNoTracking()
      .Include(x => x.Parent)
      .WhereWorld(_context.WorldId)
      .Where(x => x.Id == id)
      .SingleOrDefaultAsync(cancellationToken);
    return lineage is null ? null : await MapAsync(lineage, cancellationToken);
  }

  public async Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Lineages.Table).SelectAll(GameDb.Lineages.Table)
      .ApplyIdFilter(GameDb.Lineages.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Lineages.Name, GameDb.Lineages.Summary);

    IQueryable<LineageEntity> query = _lineages.FromQuery(builder).AsNoTracking()
      .Include(x => x.Parent)
      .WhereWorld(_context.WorldId);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<LineageEntity>? ordered = null;
    foreach (LineageSortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        LineageSort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        LineageSort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        LineageSort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The lineage sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    LineageEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<LineageModel> lineages = await MapAsync(entities, cancellationToken);

    return new SearchResults<LineageModel>(lineages, total);
  }

  private async Task<LineageModel> MapAsync(LineageEntity lineage, CancellationToken cancellationToken)
  {
    return (await MapAsync([lineage], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LineageModel>> MapAsync(IEnumerable<LineageEntity> lineages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = lineages.SelectMany(lineage => lineage.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return lineages.Select(mapper.ToLineage).ToList().AsReadOnly();
  }
}
