using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Specializations;

namespace SkillCraft.Api.Core.Specializations.Queries;

internal record SearchSpecializationsQuery(SearchSpecializationsPayload Payload) : IQuery<SearchResults<SpecializationModel>>;

internal class SearchSpecializationsQueryHandler : IQueryHandler<SearchSpecializationsQuery, SearchResults<SpecializationModel>>
{
  private readonly ISpecializationQuerier _specializationQuerier;

  public SearchSpecializationsQueryHandler(ISpecializationQuerier specializationQuerier)
  {
    _specializationQuerier = specializationQuerier;
  }

  public async Task<SearchResults<SpecializationModel>> HandleAsync(SearchSpecializationsQuery query, CancellationToken cancellationToken)
  {
    return await _specializationQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
