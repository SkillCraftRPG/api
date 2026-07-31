using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Lineages.Models;

namespace SkillCraft.Api.Core.Lineages.Queries;

internal record SearchLineagesQuery(SearchLineagesPayload Payload) : IQuery<SearchResults<LineageModel>>;

internal class SearchLineagesQueryHandler : IQueryHandler<SearchLineagesQuery, SearchResults<LineageModel>>
{
  private readonly ILineageRepository _lineageRepository;

  public SearchLineagesQueryHandler(ILineageRepository lineageRepository)
  {
    _lineageRepository = lineageRepository;
  }

  public async Task<SearchResults<LineageModel>> HandleAsync(SearchLineagesQuery query, CancellationToken cancellationToken)
  {
    return await _lineageRepository.SearchAsync(query.Payload, cancellationToken);
  }
}
