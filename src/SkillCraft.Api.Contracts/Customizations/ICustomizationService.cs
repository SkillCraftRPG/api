using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Customizations;

public interface ICustomizationService
{
  Task<CreateOrReplaceCustomizationResult> CreateOrReplaceAsync(CreateOrReplaceCustomizationPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<CustomizationModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<CustomizationModel>> SearchAsync(SearchCustomizationsPayload payload, CancellationToken cancellationToken = default);
  Task<CustomizationModel?> UpdateAsync(Guid id, UpdateCustomizationPayload payload, CancellationToken cancellationToken = default);
}
