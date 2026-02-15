using Krakenar.Contracts.Search;
using Logitar.CQRS;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Core.Customizations.Queries;

internal record SearchCustomizationsQuery(SearchCustomizationsPayload Payload) : IQuery<SearchResults<CustomizationModel>>;

internal class SearchCustomizationsQueryHandler : IQueryHandler<SearchCustomizationsQuery, SearchResults<CustomizationModel>>
{
  private readonly ICustomizationQuerier _customizationQuerier;

  public SearchCustomizationsQueryHandler(ICustomizationQuerier customizationQuerier)
  {
    _customizationQuerier = customizationQuerier;
  }

  public async Task<SearchResults<CustomizationModel>> HandleAsync(SearchCustomizationsQuery query, CancellationToken cancellationToken)
  {
    return await _customizationQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
