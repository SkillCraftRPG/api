using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Educations.Events;
using SkillCraft.Api.Core.Educations.Models;

namespace SkillCraft.Api.Core.Educations;

public interface IEducationRepository
{
  void Add(params Education[] educations);
  void Remove(Education education);
  void Update(Education education, EducationUpdated record);

  Task<Education?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<EducationModel> ReadAsync(Education education, CancellationToken cancellationToken = default);
  Task<EducationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<EducationModel>> SearchAsync(SearchEducationsPayload payload, CancellationToken cancellationToken = default);
}
