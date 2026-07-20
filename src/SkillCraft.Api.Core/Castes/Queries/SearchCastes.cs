using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Castes.Models;

namespace SkillCraft.Api.Core.Castes.Queries;

internal record SearchCastesQuery(SearchCastesPayload Payload) : IQuery<SearchResults<CasteModel>>;

internal class SearchCastesQueryHandler : IQueryHandler<SearchCastesQuery, SearchResults<CasteModel>>
{
  private readonly ICasteRepository _casteRepository;

  public SearchCastesQueryHandler(ICasteRepository casteRepository)
  {
    _casteRepository = casteRepository;
  }

  public async Task<SearchResults<CasteModel>> HandleAsync(SearchCastesQuery query, CancellationToken cancellationToken)
  {
    return await _casteRepository.SearchAsync(query.Payload, cancellationToken);
  }
}
