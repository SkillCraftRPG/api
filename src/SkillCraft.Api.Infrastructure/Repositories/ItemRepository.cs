using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Items;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class ItemRepository : Logitar.EventSourcing.Repository, IItemRepository
{
  private readonly GameContext _database;

  public ItemRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Item?> LoadAsync(ItemId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Item>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Item>> LoadAsync(IEnumerable<ItemId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Item>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Item item, CancellationToken cancellationToken)
  {
    await base.SaveAsync(item, cancellationToken);

    await SynchronizeAsync(item, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Item> items, CancellationToken cancellationToken)
  {
    await base.SaveAsync(items, cancellationToken);

    foreach (Item item in items)
    {
      await SynchronizeAsync(item, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Item item, CancellationToken cancellationToken)
  {
    ItemEntity? entity = await _database.Items.SingleOrDefaultAsync(x => x.StreamId == item.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new ItemEntity(item);
      _database.Items.Add(entity);
    }
    else
    {
      entity.Update(item);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
