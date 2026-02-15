using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Castes;

public interface ICasteService
{
  Task<CreateOrReplaceCasteResult> CreateOrReplaceAsync(CreateOrReplaceCastePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<CasteModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<CasteModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<CasteModel>> SearchAsync(SearchCastesPayload payload, CancellationToken cancellationToken = default);
  Task<CasteModel?> UpdateAsync(Guid id, UpdateCastePayload payload, CancellationToken cancellationToken = default);
}
