using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Educations;

namespace SkillCraft.Api.Core.Educations;

public interface IEducationQuerier
{
  Task<EducationModel> ReadAsync(Education education, CancellationToken cancellationToken = default);
  Task<EducationModel?> ReadAsync(EducationId id, CancellationToken cancellationToken = default);
  Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<EducationModel>> SearchAsync(SearchEducationsPayload payload, CancellationToken cancellationToken = default);
}
