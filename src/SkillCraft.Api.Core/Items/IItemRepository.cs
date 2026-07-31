using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Items.Events;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Core.Items;

public interface IItemRepository
{
  void Add(params Item[] items);
  void Remove(Item item);
  void Update(Item item, ItemUpdated record);

  Task<Item?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<ItemModel> ReadAsync(Item item, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken = default);
}
