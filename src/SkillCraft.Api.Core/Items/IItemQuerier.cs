using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Core.Items;

public interface IItemQuerier
{
  Task<ItemModel> ReadAsync(Item item, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(ItemId id, CancellationToken cancellationToken = default);
  Task<ItemModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<ItemModel>> SearchAsync(SearchItemsPayload payload, CancellationToken cancellationToken = default);
}
