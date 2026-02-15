using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts.Worlds;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Worlds;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class WorldQuerier : IWorldQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<WorldEntity> _worlds;

  public WorldQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
    _worlds = game.Worlds;
  }

  public async Task<int> CountAsync(CancellationToken cancellationToken)
  {
    return await _worlds.AsNoTracking()
      .Where(x => x.OwnerId == _context.UserId.Value)
      .CountAsync(cancellationToken);
  }

  public async Task<WorldModel> ReadAsync(World world, CancellationToken cancellationToken)
  {
    return await ReadAsync(world.Id, cancellationToken) ?? throw new InvalidOperationException($"The world entity 'StreamId={world.Id}' was not found.");
  }
  public async Task<WorldModel?> ReadAsync(WorldId id, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _worlds.AsNoTracking()
      .Where(x => _context.IsAdministrator || x.OwnerId == _context.UserId.Value)
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return world is null ? null : await MapAsync(world, cancellationToken);
  }
  public async Task<WorldModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    WorldEntity? world = await _worlds.AsNoTracking()
      .Where(x => _context.IsAdministrator || x.OwnerId == _context.UserId.Value)
      .Where(x => x.Id == id)
      .SingleOrDefaultAsync(cancellationToken);
    return world is null ? null : await MapAsync(world, cancellationToken);
  }

  public async Task<SearchResults<WorldModel>> SearchAsync(SearchWorldsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Worlds.Table).SelectAll(GameDb.Worlds.Table)
      .ApplyIdFilter(GameDb.Worlds.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Worlds.Name);

    IQueryable<WorldEntity> query = _worlds.FromQuery(builder).AsNoTracking()
      .Where(x => _context.IsAdministrator || x.OwnerId == _context.UserId.Value);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<WorldEntity>? ordered = null;
    foreach (WorldSortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        WorldSort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        WorldSort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        WorldSort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The world sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    WorldEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<WorldModel> worlds = await MapAsync(entities, cancellationToken);

    return new SearchResults<WorldModel>(worlds, total);
  }

  private async Task<WorldModel> MapAsync(WorldEntity world, CancellationToken cancellationToken)
  {
    return (await MapAsync([world], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<WorldModel>> MapAsync(IEnumerable<WorldEntity> worlds, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = worlds.SelectMany(world => world.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return worlds.Select(mapper.ToWorld).ToList().AsReadOnly();
  }
}
