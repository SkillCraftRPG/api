using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Lineages;

namespace SkillCraft.Api.Core.Lineages;

public interface ILineageQuerier
{
  Task<bool> HasChildrenAsync(Lineage lineage, CancellationToken cancellationToken = default);

  Task<LineageModel> ReadAsync(Lineage lineage, CancellationToken cancellationToken = default);
  Task<LineageModel?> ReadAsync(LineageId id, CancellationToken cancellationToken = default);
  Task<LineageModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<LineageModel>> SearchAsync(SearchLineagesPayload payload, CancellationToken cancellationToken = default);
}
