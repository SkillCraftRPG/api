namespace SkillCraft.Api.Core.Parties;

public interface IPartyRepository
{
  Task<Party?> LoadAsync(PartyId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Party>> LoadAsync(IEnumerable<PartyId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Party party, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Party> parties, CancellationToken cancellationToken = default);
}
