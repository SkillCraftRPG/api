using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Core.Spells.Events;
using SkillCraft.Api.Core.Spells.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class SpellRepository : Repository, ISpellRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public SpellRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Spell[] spells)
  {
    foreach (Spell spell in spells)
    {
      Database.Spells.Add(spell);
      base.RecordChange(new SpellCreated(spell));
    }
  }
  public void Remove(Spell spell)
  {
    Database.Spells.Remove(spell);
    base.RecordChange(new SpellDeleted(spell, _context.UserId));
  }
  public void Update(Spell spell, SpellUpdated record)
  {
    Database.Spells.Update(spell);
    base.RecordChange(record);
  }

  public async Task<Spell?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Spells.SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<SpellModel> ReadAsync(Spell spell, CancellationToken cancellationToken)
  {
    return await ReadAsync(spell.Id, cancellationToken) ?? throw new InvalidOperationException($"The spell 'Id={spell.Id}' was not found.");
  }
  public async Task<SpellModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Spell? spell = await Database.Spells.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .SingleOrDefaultAsync(cancellationToken);

    return spell is null ? null : await MapAsync(spell, cancellationToken);
  }

  public virtual async Task<SearchResults<SpellModel>> SearchAsync(SearchSpellsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Spells.Table).SelectAll(Db.Spells.Table)
      .Where(Db.Spells.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Spells.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Spells.Name, Db.Spells.Summary);

    if (payload.Tiers.Count > 0)
    {
      builder.Where(Db.Spells.Tier, Operators.IsIn(payload.Tiers.Select(tier => (object)tier).ToArray()));
    }

    IQueryable<Spell> query = Database.Spells.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Spell>? ordered = null;
    foreach (SpellSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case SpellSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case SpellSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case SpellSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    Spell[] spells = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<SpellModel> items = await MapAsync(spells, cancellationToken);

    return new SearchResults<SpellModel>(items, total);
  }

  private async Task<SpellModel> MapAsync(Spell spell, CancellationToken cancellationToken)
  {
    return (await MapAsync([spell], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<SpellModel>> MapAsync(IEnumerable<Spell> spells, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = spells.SelectMany(spell => spell.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return spells.Select(mapper.ToSpell).ToList().AsReadOnly();
  }
}
