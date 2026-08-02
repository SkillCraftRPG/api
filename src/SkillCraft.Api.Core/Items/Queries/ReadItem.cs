using Logitar.CQRS;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Core.Items.Queries;

internal record ReadItemQuery(Guid Id) : IQuery<ItemModel?>;

internal class ReadItemQueryHandler : IQueryHandler<ReadItemQuery, ItemModel?>
{
  private readonly IItemQuerier _itemQuerier;

  public ReadItemQueryHandler(IItemQuerier itemQuerier)
  {
    _itemQuerier = itemQuerier;
  }

  public async Task<ItemModel?> HandleAsync(ReadItemQuery query, CancellationToken cancellationToken)
  {
    return await _itemQuerier.ReadAsync(query.Id, cancellationToken);
  }
}
