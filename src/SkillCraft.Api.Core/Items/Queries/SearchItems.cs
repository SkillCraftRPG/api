using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Items.Models;

namespace SkillCraft.Api.Core.Items.Queries;

internal record SearchItemsQuery(SearchItemsPayload Payload) : IQuery<SearchResults<ItemModel>>;

internal class SearchItemsQueryHandler : IQueryHandler<SearchItemsQuery, SearchResults<ItemModel>>
{
  private readonly IItemQuerier _itemQuerier;

  public SearchItemsQueryHandler(IItemQuerier itemQuerier)
  {
    _itemQuerier = itemQuerier;
  }

  public async Task<SearchResults<ItemModel>> HandleAsync(SearchItemsQuery query, CancellationToken cancellationToken)
  {
    return await _itemQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
