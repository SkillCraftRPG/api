using Krakenar.Contracts.Search;
using SkillCraft.Api.Core.Castes.Events;
using SkillCraft.Api.Core.Castes.Models;

namespace SkillCraft.Api.Core.Castes;

public interface ICasteRepository
{
  void Add(params Caste[] castes);
  void Remove(Caste caste);
  void Update(Caste caste, CasteUpdated record);

  Task<Caste?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<CasteModel> ReadAsync(Caste caste, CancellationToken cancellationToken = default);
  Task<CasteModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<CasteModel>> SearchAsync(SearchCastesPayload payload, CancellationToken cancellationToken = default);
}
