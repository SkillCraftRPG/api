using Logitar.EventSourcing;
using SkillCraft.Api.Core.Items;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class ItemRepository : Repository, IItemRepository
{
  public ItemRepository(IEventStore eventStore) : base(eventStore)
  {
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
  }
  public async Task SaveAsync(IEnumerable<Item> items, CancellationToken cancellationToken)
  {
    await base.SaveAsync(items, cancellationToken);
  }
}
