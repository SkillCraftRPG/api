using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Core.Customizations.Models;

namespace SkillCraft.Api.Core.Customizations.Queries;

internal record SearchCustomizationsQuery(SearchCustomizationsPayload Payload) : IQuery<SearchResults<CustomizationModel>>;

internal class SearchCustomizationsQueryHandler : IQueryHandler<SearchCustomizationsQuery, SearchResults<CustomizationModel>>
{
  private readonly ICustomizationRepository _customizationRepository;

  public SearchCustomizationsQueryHandler(ICustomizationRepository customizationRepository)
  {
    _customizationRepository = customizationRepository;
  }

  public async Task<SearchResults<CustomizationModel>> HandleAsync(SearchCustomizationsQuery query, CancellationToken cancellationToken)
  {
    return await _customizationRepository.SearchAsync(query.Payload, cancellationToken);
  }
}
