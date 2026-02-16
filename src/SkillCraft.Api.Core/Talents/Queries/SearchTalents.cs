using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Talents;

namespace SkillCraft.Api.Core.Talents.Queries;

internal record SearchTalentsQuery(SearchTalentsPayload Payload) : IQuery<SearchResults<TalentModel>>;

internal class SearchTalentsQueryHandler : IQueryHandler<SearchTalentsQuery, SearchResults<TalentModel>>
{
  private readonly ITalentQuerier _talentQuerier;

  public SearchTalentsQueryHandler(ITalentQuerier talentQuerier)
  {
    _talentQuerier = talentQuerier;
  }

  public async Task<SearchResults<TalentModel>> HandleAsync(SearchTalentsQuery query, CancellationToken cancellationToken)
  {
    return await _talentQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
