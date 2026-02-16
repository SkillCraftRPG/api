using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Contracts;
using SkillCraft.Api.Contracts.Talents;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class TalentQuerier : ITalentQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<TalentEntity> _talents;
  private readonly ISqlHelper _sqlHelper;

  public TalentQuerier(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _talents = game.Talents;
    _sqlHelper = sqlHelper;
  }

  public async Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken)
  {
    return await ReadAsync(talent.Id, cancellationToken) ?? throw new InvalidOperationException($"The talent entity 'StreamId={talent.Id}' was not found.");
  }
  public async Task<TalentModel?> ReadAsync(TalentId id, CancellationToken cancellationToken)
  {
    TalentEntity? talent = await _talents.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.StreamId == id.Value)
      .Include(x => x.RequiredTalent)
      .SingleOrDefaultAsync(cancellationToken);
    return talent is null ? null : await MapAsync(talent, cancellationToken);
  }
  public async Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TalentEntity? talent = await _talents.AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Where(x => x.Id == id)
      .Include(x => x.RequiredTalent)
      .SingleOrDefaultAsync(cancellationToken);
    return talent is null ? null : await MapAsync(talent, cancellationToken);
  }

  public async Task<SearchResults<TalentModel>> SearchAsync(SearchTalentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(GameDb.Talents.Table).SelectAll(GameDb.Talents.Table)
      .ApplyIdFilter(GameDb.Talents.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, GameDb.Talents.Name, GameDb.Talents.Summary);

    if (payload.Tiers.Count > 0)
    {
      object[] tiers = payload.Tiers.Distinct().Select(tier => (object)tier).ToArray();
      builder.Where(GameDb.Talents.Tier, Operators.IsIn(tiers));
    }
    if (payload.AllowMultiplePurchases.HasValue)
    {
      builder.Where(GameDb.Talents.AllowMultiplePurchases, Operators.IsEqualTo(payload.AllowMultiplePurchases.Value));
    }
    if (!string.IsNullOrWhiteSpace(payload.Skill))
    {
      string skill = payload.Skill.Trim();
      if (Enum.TryParse(skill, out GameSkill skillValue))
      {
        builder.Where(GameDb.Talents.Skill, Operators.IsEqualTo(skillValue.ToString()));
      }
      else if (skill.Equals("any", StringComparison.InvariantCultureIgnoreCase))
      {
        builder.Where(GameDb.Talents.Skill, Operators.IsNotNull());
      }
      else if (skill.Equals("none", StringComparison.InvariantCultureIgnoreCase))
      {
        builder.Where(GameDb.Talents.Skill, Operators.IsNull());
      }
    }
    if (payload.RequiredTalent is not null)
    {
      string requiredTalent = payload.RequiredTalent.Trim();
      if (Guid.TryParse(requiredTalent, out Guid id))
      {
        builder.Where(GameDb.Talents.RequiredTalentUid, Operators.IsEqualTo(id));
      }
      else if (requiredTalent.Equals("any", StringComparison.InvariantCultureIgnoreCase))
      {
        builder.Where(GameDb.Talents.RequiredTalentId, Operators.IsNotNull());
      }
      else if (requiredTalent.Equals("none", StringComparison.InvariantCultureIgnoreCase))
      {
        builder.Where(GameDb.Talents.RequiredTalentId, Operators.IsNull());
      }
    }

    IQueryable<TalentEntity> query = _talents.FromQuery(builder).AsNoTracking()
      .WhereWorld(_context.WorldId)
      .Include(x => x.RequiredTalent);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<TalentEntity>? ordered = null;
    foreach (TalentSortOption sort in payload.Sort)
    {
      ordered = sort.Field switch
      {
        TalentSort.CreatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn)),
        TalentSort.Name => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name)),
        TalentSort.UpdatedOn => ordered is null
          ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
          : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn)),
        _ => throw new NotSupportedException($"The talent sort field '{sort.Field}' is not supported."),
      };
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    TalentEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<TalentModel> talents = await MapAsync(entities, cancellationToken);

    return new SearchResults<TalentModel>(talents, total);
  }

  private async Task<TalentModel> MapAsync(TalentEntity talent, CancellationToken cancellationToken)
  {
    return (await MapAsync([talent], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<TalentModel>> MapAsync(IEnumerable<TalentEntity> talents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = talents.SelectMany(talent => talent.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    GameMapper mapper = new(actors);

    return talents.Select(mapper.ToTalent).ToList().AsReadOnly();
  }
}
