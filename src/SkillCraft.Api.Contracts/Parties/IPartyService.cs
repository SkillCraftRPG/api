using Krakenar.Contracts.Search;

namespace SkillCraft.Api.Contracts.Parties;

public interface IPartyService
{
  Task<CreateOrReplacePartyResult> CreateOrReplaceAsync(CreateOrReplacePartyPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<PartyModel?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PartyModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<PartyModel>> SearchAsync(SearchPartiesPayload payload, CancellationToken cancellationToken = default);
  Task<PartyModel?> UpdateAsync(Guid id, UpdatePartyPayload payload, CancellationToken cancellationToken = default);
}
