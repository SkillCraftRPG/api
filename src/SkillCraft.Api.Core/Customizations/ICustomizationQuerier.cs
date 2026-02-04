using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Customizations;

namespace SkillCraft.Api.Core.Customizations;

public interface ICustomizationQuerier
{
  Task<CustomizationModel> ReadAsync(Customization customization, CancellationToken cancellationToken = default);
  Task<CustomizationModel?> ReadAsync(CustomizationId id, CancellationToken cancellationToken = default);
  Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<CustomizationModel>> SearchAsync(SearchCustomizationsPayload payload, CancellationToken cancellationToken = default);
}
