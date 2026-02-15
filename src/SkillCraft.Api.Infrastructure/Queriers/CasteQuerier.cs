using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts.Castes;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class CasteQuerier : ICasteQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<CasteEntity> _castes;
  private readonly ISqlHelper _sqlHelper;

  public CasteQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _castes = game.Castes;
    _sqlHelper = sqlHelper;
  }

  public async Task<CasteModel> ReadAsync(Caste caste, CancellationToken cancellationToken)
  {
    return await ReadAsync(caste.Id, cancellationToken) ?? throw new InvalidOperationException($"The caste entity 'StreamId={caste.Id}' was not found.");
  }
  public async Task<CasteModel?> ReadAsync(CasteId id, CancellationToken cancellationToken)
  {
    CasteEntity? caste = await _castes.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return caste is null ? null : await MapAsync(caste, cancellationToken);
  }
  public async Task<CasteModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    CasteEntity? caste = await _castes.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.Id == id)
      .SingleOrDefaultAsync(cancellationToken);
    return caste is null ? null : await MapAsync(caste, cancellationToken);
  }

  public async Task<SearchResults<CasteModel>> SearchAsync(SearchCastesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Castes.Table).SelectAll(GameDb.Castes.Table)
      .ApplyIdFilter(GameDb.Castes.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Castes.Name, GameDb.Castes.Summary);

    if (payload.Skill.HasValue)
    {
      builder.Where(GameDb.Castes.Skill, Operators.IsEqualTo(payload.Skill.Value.ToString()));
    }

    IQueryable<CasteEntity> query = _castes.FromQuery(builder).AsNoTracking()
      .WhereWorld(_context.WorldId);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<CasteEntity>? ordered = null;
    foreach (CasteSortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        CasteSort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        CasteSort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        CasteSort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The caste sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    CasteEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<CasteModel> castes = await MapAsync(entities, cancellationToken);

    return new SearchResults<CasteModel>(castes, total);
  }

  private async Task<CasteModel> MapAsync(CasteEntity caste, CancellationToken cancellationToken)
  {
    return (await MapAsync([caste], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<CasteModel>> MapAsync(IEnumerable<CasteEntity> castes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = castes.SelectMany(caste => caste.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return castes.Select(mapper.ToCaste).ToList().AsReadOnly();
  }
}
