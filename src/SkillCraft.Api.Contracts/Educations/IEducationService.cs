using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Educations;

public interface IEducationService
{
  Task<CreateOrReplaceEducationResult> CreateOrReplaceAsync(CreateOrReplaceEducationPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<EducationModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<EducationModel>> SearchAsync(SearchEducationsPayload payload, CancellationToken cancellationToken = default);
  Task<EducationModel?> UpdateAsync(Guid id, UpdateEducationPayload payload, CancellationToken cancellationToken = default);
}
