using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts.Specializations;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Specializations;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class SpecializationQuerier : ISpecializationQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<SpecializationEntity> _specializations;
  private readonly ISqlHelper _sqlHelper;

  public SpecializationQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _specializations = game.Specializations;
    _sqlHelper = sqlHelper;
  }

  public async Task<SpecializationModel> ReadAsync(Specialization specialization, CancellationToken cancellationToken)
  {
    return await ReadAsync(specialization.Id, cancellationToken) ?? throw new InvalidOperationException($"The specialization entity 'StreamId={specialization.Id}' was not found.");
  }
  public async Task<SpecializationModel?> ReadAsync(SpecializationId id, CancellationToken cancellationToken)
  {
    SpecializationEntity? specialization = await _specializations.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.StreamId == id.Value)
      .Include(x => x.OptionalTalents).ThenInclude(x => x.Talent)
      .Include(x => x.RequiredTalent)
      .SingleOrDefaultAsync(cancellationToken);
    return specialization is null ? null : await MapAsync(specialization, cancellationToken);
  }
  public async Task<SpecializationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SpecializationEntity? specialization = await _specializations.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.Id == id)
      .Include(x => x.OptionalTalents).ThenInclude(x => x.Talent)
      .Include(x => x.RequiredTalent)
      .SingleOrDefaultAsync(cancellationToken);
    return specialization is null ? null : await MapAsync(specialization, cancellationToken);
  }

  public async Task<SearchResults<SpecializationModel>> SearchAsync(SearchSpecializationsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Specializations.Table).SelectAll(GameDb.Specializations.Table)
      .ApplyIdFilter(GameDb.Specializations.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Specializations.Name, GameDb.Specializations.Summary);

    if (payload.Tiers.Count > 0)
    {
      object[] tiers = payload.Tiers.Distinct().Select(tier => (object)tier).ToArray();
      builder.Where(GameDb.Specializations.Tier, Operators.IsIn(tiers));
    }
    if (payload.RequiredTalent is not null)
    {
      string requiredTalent = payload.RequiredTalent.Trim();
      if (Guid.TryParse(requiredTalent, out Guid id))
      {
        builder.Where(GameDb.Specializations.RequiredTalentUid, Operators.IsEqualTo(id));
      }
      else if (requiredTalent.Equals("any", StringComparison.InvariantCultureIgnoreCase))
      {
        builder.Where(GameDb.Specializations.RequiredTalentId, Operators.IsNotNull());
      }
      else if (requiredTalent.Equals("none", StringComparison.InvariantCultureIgnoreCase))
      {
        builder.Where(GameDb.Specializations.RequiredTalentId, Operators.IsNull());
      }
    }

    IQueryable<SpecializationEntity> query = _specializations.FromQuery(builder).AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Include(x => x.RequiredTalent);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<SpecializationEntity>? ordered = null;
    foreach (SpecializationSortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        SpecializationSort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        SpecializationSort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        SpecializationSort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The specialization sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    SpecializationEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<SpecializationModel> specializations = await MapAsync(entities, cancellationToken);

    return new SearchResults<SpecializationModel>(specializations, total);
  }

  private async Task<SpecializationModel> MapAsync(SpecializationEntity specialization, CancellationToken cancellationToken)
  {
    return (await MapAsync([specialization], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<SpecializationModel>> MapAsync(IEnumerable<SpecializationEntity> specializations, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = specializations.SelectMany(s => s.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return specializations.Select(mapper.ToSpecialization).ToList().AsReadOnly();
  }
}
