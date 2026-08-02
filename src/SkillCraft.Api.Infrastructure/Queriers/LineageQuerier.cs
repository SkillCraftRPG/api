using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class LineageQuerier : ILineageQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly GameContext _database;
  private readonly DbSet<LineageEntity> _lineages;
  private readonly ISqlHelper _sqlHelper;

  public LineageQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _database = database;
    _lineages = database.Lineages;
    _sqlHelper = sqlHelper;
  }

  public async Task<LineageModel> ReadAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    return await ReadAsync(lineage.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The lineage entity 'StreamId={lineage.Id}' was not found.");
  }
  public async Task<LineageModel?> ReadAsync(LineageId id, CancellationToken cancellationToken)
  {
    LineageEntity? lineage = await _lineages.AsNoTracking().AsSplitQuery()
      .Where(x => x.StreamId == id.Value)
      .Include(x => x.Features)
      .Include(x => x.Languages)
      .Include(x => x.Parent).ThenInclude(x => x!.Features)
      .Include(x => x.Parent).ThenInclude(x => x!.Languages)
      .SingleOrDefaultAsync(cancellationToken);

    return lineage is null ? null : await MapAsync(lineage, cancellationToken);
  }
  public async Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    LineageEntity? lineage = await _lineages.AsNoTracking().AsSplitQuery()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId.ResourceId)
      .Include(x => x.Features)
      .Include(x => x.Languages)
      .Include(x => x.Parent).ThenInclude(x => x!.Features)
      .Include(x => x.Parent).ThenInclude(x => x!.Languages)
      .SingleOrDefaultAsync(cancellationToken);

    return lineage is null ? null : await MapAsync(lineage, cancellationToken);
  }

  public virtual async Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Lineages.Table).SelectAll(Db.Lineages.Table)
      .Where(Db.Lineages.WorldId, Operators.IsEqualTo(_context.WorldId.ResourceId))
      .ApplyIdFilter(Db.Lineages.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Lineages.Name, Db.Lineages.Summary);

    if (payload.ParentId.HasValue)
    {
      TableId parentLineages = new(Db.Lineages.Table.Schema, Db.Lineages.Table.Table!, "ParentLineage");
      ColumnId parentLineageId = new(nameof(LineageEntity.LineageId), parentLineages);
      ColumnId parentLineageUid = new(nameof(LineageEntity.Id), parentLineages);
      OperatorCondition condition = new(parentLineageUid, Operators.IsEqualTo(payload.ParentId.Value));
      builder.Join(parentLineageId, Db.Lineages.ParentId, condition);
    }
    else
    {
      builder.Where(Db.Lineages.ParentId, Operators.IsNull());
    }

    if (payload.SizeCategory.HasValue)
    {
      builder.Where(Db.Lineages.SizeCategory, Operators.IsEqualTo(payload.SizeCategory.Value.ToString()));
    }

    IQueryable<LineageEntity> query = _lineages.FromQuery(builder).AsNoTracking().AsSplitQuery()
      .Include(x => x.Features)
      .Include(x => x.Languages);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<LineageEntity>? ordered = null;
    foreach (LineageSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case LineageSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case LineageSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case LineageSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    LineageEntity[] lineages = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<LineageModel> models = await MapAsync(lineages, cancellationToken);

    return new SearchResults<LineageModel>(models, total);
  }

  private async Task<LineageModel> MapAsync(LineageEntity lineage, CancellationToken cancellationToken)
  {
    return (await MapAsync([lineage], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LineageModel>> MapAsync(IEnumerable<LineageEntity> lineages, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = lineages.SelectMany(lineage => lineage.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return lineages.Select(mapper.ToLineage).ToList().AsReadOnly();
  }
}
