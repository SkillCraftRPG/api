using Logitar.CQRS;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Core.Items.Queries;

internal record ReadItemQuery(Guid Id) : IQuery<ItemModel?>;

internal class ReadItemQueryHandler : IQueryHandler<ReadItemQuery, ItemModel?>
{
  private readonly IItemRepository _itemRepository;

  public ReadItemQueryHandler(IItemRepository itemRepository)
  {
    _itemRepository = itemRepository;
  }

  public async Task<ItemModel?> HandleAsync(ReadItemQuery query, CancellationToken cancellationToken)
  {
    return await _itemRepository.ReadAsync(query.Id, cancellationToken);
  }
}
