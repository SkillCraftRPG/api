using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Parties;

namespace SkillCraft.Api.Core.Parties.Queries;

internal record SearchPartiesQuery(SearchPartiesPayload Payload) : IQuery<SearchResults<PartyModel>>;

internal class SearchPartiesQueryHandler : IQueryHandler<SearchPartiesQuery, SearchResults<PartyModel>>
{
  private readonly IPartyQuerier _partyQuerier;

  public SearchPartiesQueryHandler(IPartyQuerier partyQuerier)
  {
    _partyQuerier = partyQuerier;
  }

  public async Task<SearchResults<PartyModel>> HandleAsync(SearchPartiesQuery query, CancellationToken cancellationToken)
  {
    return await _partyQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
