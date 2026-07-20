using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Talents.Models;

namespace SkillCraft.Api.Core.Talents.Queries;

internal record SearchTalentsQuery(SearchTalentsPayload Payload) : IQuery<SearchResults<TalentModel>>;

internal class SearchTalentsQueryHandler : IQueryHandler<SearchTalentsQuery, SearchResults<TalentModel>>
{
  private readonly ITalentRepository _talentRepository;

  public SearchTalentsQueryHandler(ITalentRepository talentRepository)
  {
    _talentRepository = talentRepository;
  }

  public async Task<SearchResults<TalentModel>> HandleAsync(SearchTalentsQuery query, CancellationToken cancellationToken)
  {
    return await _talentRepository.SearchAsync(query.Payload, cancellationToken);
  }
}
