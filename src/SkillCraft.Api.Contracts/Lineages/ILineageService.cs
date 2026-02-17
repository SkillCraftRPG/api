using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Lineages;

public interface ILineageService
{
  Task<CreateOrReplaceLineageResult> CreateOrReplaceAsync(CreateOrReplaceLineagePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<LineageModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken = default);
  Task<LineageModel?> UpdateAsync(Guid id, UpdateLineagePayload payload, CancellationToken cancellationToken = default);
}
