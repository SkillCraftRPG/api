using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts.Parties;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Parties;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class PartyQuerier : IPartyQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<PartyEntity> _parties;
  private readonly ISqlHelper _sqlHelper;

  public PartyQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _parties = game.Parties;
    _sqlHelper = sqlHelper;
  }

  public async Task<PartyModel> ReadAsync(Party party, CancellationToken cancellationToken)
  {
    return await ReadAsync(party.Id, cancellationToken) ?? throw new InvalidOperationException($"The party entity 'StreamId={party.Id}' was not found.");
  }
  public async Task<PartyModel?> ReadAsync(PartyId id, CancellationToken cancellationToken)
  {
    PartyEntity? party = await _parties.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return party is null ? null : await MapAsync(party, cancellationToken);
  }
  public async Task<PartyModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PartyEntity? party = await _parties.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.Id == id)
      .SingleOrDefaultAsync(cancellationToken);
    return party is null ? null : await MapAsync(party, cancellationToken);
  }

  public async Task<SearchResults<PartyModel>> SearchAsync(SearchPartiesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Parties.Table).SelectAll(GameDb.Parties.Table)
      .ApplyIdFilter(GameDb.Parties.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Parties.Name);

    IQueryable<PartyEntity> query = _parties.FromQuery(builder).AsNoTracking()
      .WhereWorld(_context.WorldId);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<PartyEntity>? ordered = null;
    foreach (PartySortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        PartySort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        PartySort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        PartySort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The party sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    PartyEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<PartyModel> parties = await MapAsync(entities, cancellationToken);

    return new SearchResults<PartyModel>(parties, total);
  }

  private async Task<PartyModel> MapAsync(PartyEntity party, CancellationToken cancellationToken)
  {
    return (await MapAsync([party], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<PartyModel>> MapAsync(IEnumerable<PartyEntity> parties, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = parties.SelectMany(party => party.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return parties.Select(mapper.ToParty).ToList().AsReadOnly();
  }
}
