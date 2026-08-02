using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Spells;
using SkillCraft.Api.Core.Spells.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class SpellQuerier : ISpellQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;
  private readonly DbSet<SpellEntity> _spells;

  public SpellQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
    _spells = database.Spells;
  }

  public async Task<SpellModel> ReadAsync(Spell spell, CancellationToken cancellationToken)
  {
    return await ReadAsync(spell.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The spell entity 'StreamId={spell.Id}' was not found.");
  }
  public async Task<SpellModel?> ReadAsync(SpellId id, CancellationToken cancellationToken)
  {
    SpellEntity? spell = await _spells.AsNoTracking()
      .Where(x => x.StreamId == id.Value)
      .SingleOrDefaultAsync(cancellationToken);

    return spell is null ? null : await MapAsync(spell, cancellationToken);
  }
  public async Task<SpellModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    SpellEntity? spell = await _spells.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId.ResourceId)
      .SingleOrDefaultAsync(cancellationToken);

    return spell is null ? null : await MapAsync(spell, cancellationToken);
  }

  public virtual async Task<SearchResults<SpellModel>> SearchAsync(SearchSpellsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Spells.Table).SelectAll(Db.Spells.Table)
      .Where(Db.Spells.WorldId, Operators.IsEqualTo(_context.WorldId.ResourceId))
      .ApplyIdFilter(Db.Spells.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Spells.Name, Db.Spells.Summary);

    if (payload.Tiers.Count > 0)
    {
      builder.Where(Db.Spells.Tier, Operators.IsIn(payload.Tiers.Select(tier => (object)tier).ToArray()));
    }

    IQueryable<SpellEntity> query = _spells.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<SpellEntity>? ordered = null;
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

    SpellEntity[] spells = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<SpellModel> items = await MapAsync(spells, cancellationToken);

    return new SearchResults<SpellModel>(items, total);
  }

  private async Task<SpellModel> MapAsync(SpellEntity spell, CancellationToken cancellationToken)
  {
    return (await MapAsync([spell], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<SpellModel>> MapAsync(IEnumerable<SpellEntity> spells, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = spells.SelectMany(spell => spell.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return spells.Select(mapper.ToSpell).ToList().AsReadOnly();
  }
}
