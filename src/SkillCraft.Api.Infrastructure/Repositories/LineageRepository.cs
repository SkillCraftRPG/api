using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Lineages;
using SkillCraft.Api.Core.Lineages.Events;
using SkillCraft.Api.Core.Lineages.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class LineageRepository : Repository, ILineageRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public LineageRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Lineage[] lineages)
  {
    foreach (Lineage lineage in lineages)
    {
      Database.Lineages.Add(lineage);
      base.RecordChange(new LineageCreated(lineage));
    }
  }
  public void Remove(Lineage lineage)
  {
    Database.Lineages.Remove(lineage);
    base.RecordChange(new LineageDeleted(lineage, _context.UserId));
  }
  public void Update(Lineage lineage, LineageUpdated record)
  {
    Database.Lineages.Update(lineage);
    base.RecordChange(record);
  }

  public async Task<Lineage?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Lineages
      .Include(x => x.Features)
      .Include(x => x.Languages)
      .SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<LineageModel> ReadAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    return await ReadAsync(lineage.Id, cancellationToken) ?? throw new InvalidOperationException($"The lineage 'Id={lineage.Id}' was not found.");
  }
  public async Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Lineage? lineage = await Database.Lineages.AsNoTracking().AsSplitQuery()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .Include(x => x.Features)
      .Include(x => x.Languages).ThenInclude(x => x.Script)
      .Include(x => x.Parent).ThenInclude(x => x!.Features)
      .Include(x => x.Parent).ThenInclude(x => x!.Languages).ThenInclude(x => x.Script)
      .SingleOrDefaultAsync(cancellationToken);

    return lineage is null ? null : await MapAsync(lineage, cancellationToken);
  }

  public virtual async Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Lineages.Table).SelectAll(Db.Lineages.Table)
      .Where(Db.Lineages.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Lineages.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Lineages.Name, Db.Lineages.Summary);

    if (payload.ParentId.HasValue)
    {
      TableId parentLineages = new(Db.Lineages.Table.Schema, Db.Lineages.Table.Table!, "ParentLineage");
      ColumnId parentLineageId = new(nameof(Lineage.LineageId), parentLineages);
      ColumnId parentLineageUid = new(nameof(Lineage.Id), parentLineages);
      OperatorCondition condition = new(parentLineageUid, Operators.IsEqualTo(payload.ParentId.Value));
      builder.Join(parentLineageId, Db.Lineages.ParentId, condition);
    }
    if (payload.SizeCategory.HasValue)
    {
      builder.Where(Db.Lineages.SizeCategory, Operators.IsEqualTo(payload.SizeCategory.Value.ToString()));
    }

    IQueryable<Lineage> query = Database.Lineages.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Lineage>? ordered = null;
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

    Lineage[] lineages = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<LineageModel> items = await MapAsync(lineages, cancellationToken);

    return new SearchResults<LineageModel>(items, total);
  }

  private async Task<LineageModel> MapAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    return (await MapAsync([lineage], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<LineageModel>> MapAsync(IEnumerable<Lineage> lineages, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = lineages.SelectMany(lineage => lineage.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return lineages.Select(mapper.ToLineage).ToList().AsReadOnly();
  }
}
