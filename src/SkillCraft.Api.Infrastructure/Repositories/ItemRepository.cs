using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Core.Items.Events;
using SkillCraft.Api.Core.Items.Models;
using SkillCraft.Api.Infrastructure.Actors;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class ItemRepository : Repository, IItemRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly ISqlHelper _sqlHelper;

  public ItemRepository(IActorService actorService, IContext context, GameContext game, ISqlHelper sqlHelper) : base(game)
  {
    _actorService = actorService;
    _context = context;
    _sqlHelper = sqlHelper;
  }

  public void Add(params Item[] items)
  {
    foreach (Item item in items)
    {
      Database.Items.Add(item);
      base.RecordChange(new ItemCreated(item));
    }
  }
  public void Remove(Item item)
  {
    Database.Items.Remove(item);
    base.RecordChange(new ItemDeleted(item, _context.UserId));
  }
  public void Update(Item item, ItemUpdated record)
  {
    Database.Items.Update(item);
    base.RecordChange(record);
  }

  public async Task<Item?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Items.SingleOrDefaultAsync(x => x.Id == id && x.WorldId == _context.WorldId, cancellationToken);
  }

  public async Task<ItemModel> ReadAsync(Item item, CancellationToken cancellationToken)
  {
    return await ReadAsync(item.Id, cancellationToken) ?? throw new InvalidOperationException($"The item 'Id={item.Id}' was not found.");
  }
  public async Task<ItemModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Item? item = await Database.Items.AsNoTracking()
      .Where(x => x.Id == id && x.WorldId == _context.WorldId)
      .SingleOrDefaultAsync(cancellationToken);

    return item is null ? null : await MapAsync(item, cancellationToken);
  }

  public virtual async Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Items.Table).SelectAll(Db.Items.Table)
      .Where(Db.Items.WorldId, Operators.IsEqualTo(_context.WorldId))
      .ApplyIdFilter(Db.Items.Id, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Items.Name, Db.Items.Summary);

    IQueryable<Item> query = Database.Items.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<Item>? ordered = null;
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

    Item[] items = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<ItemModel> models = await MapAsync(items, cancellationToken);

    return new SearchResults<ItemModel>(models, total);
  }

  private async Task<ItemModel> MapAsync(Item item, CancellationToken cancellationToken)
  {
    return (await MapAsync([item], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<ItemModel>> MapAsync(IEnumerable<Item> items, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = items.SelectMany(item => item.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return items.Select(mapper.ToItem).ToList().AsReadOnly();
  }
}
