using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Specializations;

namespace SkillCraft.Api.Core.Specializations;

public interface ISpecializationQuerier
{
  Task<SpecializationModel> ReadAsync(Specialization specialization, CancellationToken cancellationToken = default);
  Task<SpecializationModel?> ReadAsync(SpecializationId id, CancellationToken cancellationToken = default);
  Task<SpecializationModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<SpecializationModel>> SearchAsync(SearchSpecializationsPayload payload, CancellationToken cancellationToken = default);
}
