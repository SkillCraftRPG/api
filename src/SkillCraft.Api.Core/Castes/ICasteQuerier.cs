using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Castes;

namespace SkillCraft.Api.Core.Castes;

public interface ICasteQuerier
{
  Task<CasteModel> ReadAsync(Caste caste, CancellationToken cancellationToken = default);
  Task<CasteModel?> ReadAsync(CasteId id, CancellationToken cancellationToken = default);
  Task<CasteModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<CasteModel>> SearchAsync(SearchCastesPayload payload, CancellationToken cancellationToken = default);
}
