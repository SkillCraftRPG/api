using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Talents;
using SkillCraft.Api.Core.Talents.Events;
using SkillCraft.Api.Core.Talents.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Db;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class TalentRepository : Repository, ITalentRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public TalentRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Talent[] talents)
  {
    foreach (Talent talent in talents)
    {
      Database.Talents.Add(talent);
      base.RecordChange(new TalentCreated(talent));
    }
  }
  public void Remove(Talent talent)
  {
    Database.Talents.Remove(talent);
    base.RecordChange(new TalentDeleted(talent, _context.UserId));
  }
  public void Update(Talent talent, TalentUpdated record)
  {
    Database.Talents.Update(talent);
    base.RecordChange(record);
  }

  public async Task<Talent?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Talents
      .Include(x => x.RequiredTalent)
      .SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<TalentModel> ReadAsync(Talent talent, CancellationToken cancellationToken)
  {
    return await ReadAsync(talent.Id, cancellationToken) ?? throw new InvalidOperationException($"The talent 'Id={talent.Id}' was not found.");
  }
  public async Task<TalentModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Talent? talent = await Database.Talents.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .Include(x => x.RequiredTalent)
      .SingleOrDefaultAsync(cancellationToken);

    return talent is null ? null : await MapAsync(talent, cancellationToken);
  }

  public virtual async Task<SearchResults<TalentModel>> SearchAsync(SearchTalentsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Talents.Table).SelectAll(Db.Talents.Table)
      .Where(Db.Talents.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Talents.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Talents.Name, Db.Talents.Summary);

    if (payload.Tiers.Count > 0)
    {
      builder.Where(Talents.Tier, Operators.IsIn(payload.Tiers.Select(tier => (object)tier).ToArray()));
    }
    if (payload.AllowMultiplePurchases.HasValue)
    {
      builder.Where(Talents.AllowMultiplePurchases, Operators.IsEqualTo(payload.AllowMultiplePurchases.Value));
    }
    if (payload.Skill.HasValue)
    {
      builder.Where(Talents.Skill, Operators.IsEqualTo(payload.Skill.Value.ToString()));
    }
    if (payload.RequiredTalentId.HasValue)
    {
      TableId requiredTalents = new(Talents.Table.Schema, Talents.Table.Table!, "RequiredTalents");
      ColumnId requiredTalentId = new(nameof(Talent.TalentId), requiredTalents);
      ColumnId requiredTalentUid = new(nameof(Talent.Id), requiredTalents);
      OperatorCondition condition = new(requiredTalentUid, Operators.IsEqualTo(payload.RequiredTalentId.Value));
      builder.Join(requiredTalentId, Talents.RequiredTalentId, condition);
    }

    IQueryable<Talent> query = Database.Talents.FromQuery(builder).AsNoTracking()
      .Include(x => x.RequiredTalent);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Talent>? ordered = null;
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

    Talent[] talents = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<TalentModel> items = await MapAsync(talents, cancellationToken);

    return new SearchResults<TalentModel>(items, total);
  }

  private async Task<TalentModel> MapAsync(Talent talent, CancellationToken cancellationToken)
  {
    return (await MapAsync([talent], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<TalentModel>> MapAsync(IEnumerable<Talent> talents, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = talents.SelectMany(talent => talent.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return talents.Select(mapper.ToTalent).ToList().AsReadOnly();
  }
}
