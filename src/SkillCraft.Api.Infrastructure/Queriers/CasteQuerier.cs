using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Core.Castes.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class CasteQuerier : ICasteQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<CasteEntity> _castes;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public CasteQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _castes = database.Castes;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public async Task<CasteModel> ReadAsync(Caste caste, CancellationToken cancellationToken)
  {
    return await ReadAsync(caste.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The caste entity 'StreamId={caste.Id}' was not found.");
  }
  public async Task<CasteModel?> ReadAsync(CasteId id, CancellationToken cancellationToken)
  {
    CasteEntity? caste = await _castes.AsNoTracking()
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);

    return caste is null ? null : await MapAsync(caste, cancellationToken);
  }
  public async Task<CasteModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CasteEntity? caste = await _castes.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldUid)
      .SingleOrDefaultAsync(cancellationToken);

    return caste is null ? null : await MapAsync(caste, cancellationToken);
  }

  public virtual async Task<SearchResults<CasteModel>> SearchAsync(SearchCastesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Castes.Table).SelectAll(Db.Castes.Table)
      .Where(Db.Castes.WorldId, Operators.IsEqualTo(_context.WorldUid))
      .ApplyIdFilter(Db.Castes.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Castes.Name, Db.Castes.Summary, Db.Castes.FeatureName);

    if (payload.Skill.HasValue)
    {
      builder.Where(Db.Castes.Skill, Operators.IsEqualTo(payload.Skill.Value.ToString()));
    }

    IQueryable<CasteEntity> query = _castes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CasteEntity>? ordered = null;
    foreach (CasteSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case CasteSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case CasteSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case CasteSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    CasteEntity[] castes = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<CasteModel> items = await MapAsync(castes, cancellationToken);

    return new SearchResults<CasteModel>(items, total);
  }

  private async Task<CasteModel> MapAsync(CasteEntity caste, CancellationToken cancellationToken)
  {
    return (await MapAsync([caste], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CasteModel>> MapAsync(IEnumerable<CasteEntity> castes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = castes.SelectMany(caste => caste.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return castes.Select(mapper.ToCaste).ToList().AsReadOnly();
  }
}
