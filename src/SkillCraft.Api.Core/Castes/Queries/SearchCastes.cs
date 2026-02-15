using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Castes;

namespace SkillCraft.Api.Core.Castes.Queries;

internal record SearchCastesQuery(SearchCastesPayload Payload) : IQuery<SearchResults<CasteModel>>;

internal class SearchCastesQueryHandler : IQueryHandler<SearchCastesQuery, SearchResults<CasteModel>>
{
  private readonly ICasteQuerier _casteQuerier;

  public SearchCastesQueryHandler(ICasteQuerier casteQuerier)
  {
    _casteQuerier = casteQuerier;
  }

  public async Task<SearchResults<CasteModel>> HandleAsync(SearchCastesQuery query, CancellationToken cancellationToken)
  {
    return await _casteQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
