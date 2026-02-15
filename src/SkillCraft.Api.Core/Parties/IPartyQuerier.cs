using Krakenar.Contracts.Search;
using SkillCraft.Api.Contracts.Parties;

namespace SkillCraft.Api.Core.Parties;

public interface IPartyQuerier
{
  Task<PartyModel> ReadAsync(Party party, CancellationToken cancellationToken = default);
  Task<PartyModel?> ReadAsync(PartyId id, CancellationToken cancellationToken = default);
  Task<PartyModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<PartyModel>> SearchAsync(SearchPartiesPayload payload, CancellationToken cancellationToken = default);
}
