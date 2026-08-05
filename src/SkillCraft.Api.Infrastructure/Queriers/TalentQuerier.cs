using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class TalentQuerier : ITalentQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<TalentEntity> _talents;

  public TalentQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
    _talents = database.Talents;
  }

  public async Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken)
  {
    return await ReadAsync(talent.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The talent entity 'StreamId={talent.Id}' was not found.");
  }
  public async Task<TalentModel?> ReadAsync(TalentId id, CancellationToken cancellationToken)
  {
    TalentEntity? talent = await _talents.AsNoTracking()
      .Where(x => x.StreamId == id.Value)
      .Include(x => x.RequiredTalent)
      .SingleOrDefaultAsync(cancellationToken);

    return talent is null ? null : await MapAsync(talent, cancellationToken);
  }
  public async Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TalentEntity? talent = await _talents.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId.ResourceId)
      .Include(x => x.RequiredTalent)
      .SingleOrDefaultAsync(cancellationToken);

    return talent is null ? null : await MapAsync(talent, cancellationToken);
  }

  public virtual async Task<SearchResults<TalentModel>> SearchAsync(SearchTalentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Talents.Table).SelectAll(Db.Talents.Table)
      .Where(Db.Talents.WorldId, Operators.IsEqualTo(_context.WorldId.ResourceId))
      .ApplyIdFilter(Db.Talents.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Talents.Name, Db.Talents.Summary);

    if (payload.Tiers.Count > 0)
    {
      builder.Where(Db.Talents.Tier, Operators.IsIn(payload.Tiers.Select(tier => (object)tier).ToArray()));
    }
    if (payload.AllowMultiplePurchases.HasValue)
    {
      builder.Where(Db.Talents.AllowMultiplePurchases, Operators.IsEqualTo(payload.AllowMultiplePurchases.Value));
    }
    if (!string.IsNullOrWhiteSpace(payload.Skill))
    {
      string value = payload.Skill.Trim().ToLower();
      switch (value)
      {
        case "any":
          builder.Where(Db.Talents.Skill, Operators.IsNotNull());
          break;
        case "none":
          builder.Where(Db.Talents.Skill, Operators.IsNull());
          break;
        default:
          if (Enum.TryParse(value, ignoreCase: true, out Skill skill) && Enum.IsDefined(skill))
          {
            builder.Where(Db.Talents.Skill, Operators.IsEqualTo(skill.ToString()));
          }
          break;
      }
    }
    if (payload.RequiredTalentId.HasValue)
    {
      TableId requiredTalents = new(Db.Talents.Table.Schema, Db.Talents.Table.Table!, "RequiredTalents");
      ColumnId requiredTalentId = new(nameof(TalentEntity.TalentId), requiredTalents);
      ColumnId requiredTalentUid = new(nameof(TalentEntity.Id), requiredTalents);
      OperatorCondition condition = new(requiredTalentUid, Operators.IsEqualTo(payload.RequiredTalentId.Value));
      builder.Join(requiredTalentId, Db.Talents.RequiredTalentId, condition);
    }

    IQueryable<TalentEntity> query = _talents.FromQuery(builder).AsNoTracking()
      .Include(x => x.RequiredTalent);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<TalentEntity>? ordered = null;
    foreach (TalentSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case TalentSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case TalentSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case TalentSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    TalentEntity[] talents = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<TalentModel> items = await MapAsync(talents, cancellationToken);

    return new SearchResults<TalentModel>(items, total);
  }

  private async Task<TalentModel> MapAsync(TalentEntity talent, CancellationToken cancellationToken)
  {
    return (await MapAsync([talent], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<TalentModel>> MapAsync(IEnumerable<TalentEntity> talents, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = talents.SelectMany(talent => talent.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return talents.Select(mapper.ToTalent).ToList().AsReadOnly();
  }
}
