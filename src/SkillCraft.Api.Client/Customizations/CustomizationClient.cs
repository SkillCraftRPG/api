using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Client.Customizations;

internal class CustomizationClient : ICustomizationService // TODO(fpion): CustomizationClient with RequestContext
{
  public async Task<CreateOrReplaceCustomizationResult> CreateOrReplaceAsync(CreateOrReplaceCustomizationPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    return new CreateOrReplaceCustomizationResult(new CustomizationModel(), Created: false); // TODO(fpion): implement
  }

  public async Task<CustomizationModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return null; // TODO(fpion): implement
  }

  public async Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return null; // TODO(fpion): implement
  }

  public async Task<SearchResults<CustomizationModel>> SearchAsync(SearchCustomizationsPayload payload, CancellationToken cancellationToken)
  {
    return new SearchResults<CustomizationModel>(); // TODO(fpion): implement
  }

  public async Task<CustomizationModel?> UpdateAsync(Guid id, UpdateCustomizationPayload payload, CancellationToken cancellationToken)
  {
    return null; // TODO(fpion): implement
  }
}
