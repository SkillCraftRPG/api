using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Educations.Models;

namespace SkillCraft.Api.Core.Educations.Queries;

internal record SearchEducationsQuery(SearchEducationsPayload Payload) : IQuery<SearchResults<EducationModel>>;

internal class SearchEducationsQueryHandler : IQueryHandler<SearchEducationsQuery, SearchResults<EducationModel>>
{
  private readonly IEducationRepository _educationRepository;

  public SearchEducationsQueryHandler(IEducationRepository educationRepository)
  {
    _educationRepository = educationRepository;
  }

  public async Task<SearchResults<EducationModel>> HandleAsync(SearchEducationsQuery query, CancellationToken cancellationToken)
  {
    return await _educationRepository.SearchAsync(query.Payload, cancellationToken);
  }
}
