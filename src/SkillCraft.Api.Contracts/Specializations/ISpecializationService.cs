using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Specializations;

public interface ISpecializationService
{
  Task<CreateOrReplaceSpecializationResult> CreateOrReplaceAsync(CreateOrReplaceSpecializationPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<SpecializationModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SpecializationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<SpecializationModel>> SearchAsync(SearchSpecializationsPayload payload, CancellationToken cancellationToken = default);
  Task<SpecializationModel?> UpdateAsync(Guid id, UpdateSpecializationPayload payload, CancellationToken cancellationToken = default);
}
