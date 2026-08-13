using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Items.Models;
using SkillCraft.Api.Infrastructure.Actors;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Queriers;

internal class ItemQuerier : IItemQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<ItemEntity> _items;
  private readonly ISqlHelper _sqlHelper;

  public ItemQuerier(IActorService actorService, IContext context, GameContext database, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _items = database.Items;
    _sqlHelper = sqlHelper;
  }

  public async Task<ItemModel> ReadAsync(Item item, CancellationToken cancellationToken)
  {
    return await ReadAsync(item.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The item entity 'StreamId={item.Id}' was not found.");
  }
  public async Task<ItemModel?> ReadAsync(ItemId id, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _items.AsNoTracking()
      .Where(x => x.StreamId == id.Value)
      .Include(x => x.Replacement)
      .SingleOrDefaultAsync(cancellationToken);

    return item is null ? null : await MapAsync(item, cancellationToken);
  }
  public async Task<ItemModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ItemEntity? item = await _items.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId.ResourceId)
      .Include(x => x.Replacement)
      .SingleOrDefaultAsync(cancellationToken);

    return item is null ? null : await MapAsync(item, cancellationToken);
  }

  public virtual async Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Items.Table).SelectAll(Db.Items.Table)
      .Where(Db.Items.WorldId, Operators.IsEqualTo(_context.WorldId.ResourceId))
      .ApplyIdFilter(Db.Items.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Items.Name, Db.Items.Summary);

    if (payload.Category.HasValue)
    {
      builder.Where(Db.Items.Category, Operators.IsEqualTo(payload.Category.Value.ToString()));
    }
    if (payload.Rarity.HasValue)
    {
      builder.Where(Db.Items.Rarity, Operators.IsEqualTo(payload.Rarity.Value.ToString()));
    }

    IQueryable<ItemEntity> query = _items.FromQuery(builder).AsNoTracking()
      .Include(x => x.Replacement);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<ItemEntity>? ordered = null;
    foreach (ItemSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case ItemSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case ItemSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case ItemSort.Price:
          ordered = (ordered is null)
            ? (sort.IsDescending
              ? query.OrderByDescending(x => x.Price.HasValue).ThenByDescending(x => x.Price)
              : query.OrderByDescending(x => x.Price.HasValue).ThenBy(x => x.Price))
            : (sort.IsDescending
              ? ordered.ThenByDescending(x => x.Price.HasValue).ThenByDescending(x => x.Price)
              : ordered.ThenByDescending(x => x.Price.HasValue).ThenBy(x => x.Price));
          break;
        case ItemSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
        case ItemSort.Weight:
          ordered = (ordered is null)
            ? (sort.IsDescending
              ? query.OrderByDescending(x => x.Weight.HasValue).ThenByDescending(x => x.Weight)
              : query.OrderByDescending(x => x.Weight.HasValue).ThenBy(x => x.Weight))
            : (sort.IsDescending
              ? ordered.ThenByDescending(x => x.Weight.HasValue).ThenByDescending(x => x.Weight)
              : ordered.ThenByDescending(x => x.Weight.HasValue).ThenBy(x => x.Weight));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    ItemEntity[] items = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ItemModel> models = await MapAsync(items, cancellationToken);

    return new SearchResults<ItemModel>(models, total);
  }

  private async Task<ItemModel> MapAsync(ItemEntity item, CancellationToken cancellationToken)
  {
    return (await MapAsync([item], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ItemModel>> MapAsync(IEnumerable<ItemEntity> items, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = items.SelectMany(item => item.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return items.Select(mapper.ToItem).ToList().AsReadOnly();
  }
}
