using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Customizations.Events;
using SkillCraft.Api.Core.Customizations.Models;

namespace SkillCraft.Api.Core.Customizations;

public interface ICustomizationRepository
{
  void Add(params Customization[] customizations);
  void Remove(Customization customization);
  void Update(Customization customization, CustomizationUpdated record);

  Task<Customization?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<CustomizationModel> ReadAsync(Customization customization, CancellationToken cancellationToken = default);
  Task<CustomizationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<CustomizationModel>> SearchAsync(SearchCustomizationsPayload payload, CancellationToken cancellationToken = default);
}
