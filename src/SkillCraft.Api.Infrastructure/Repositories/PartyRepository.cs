using Logitar.EventSourcing;
using SkillCraft.Api.Core.Parties;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class PartyRepository : Repository, IPartyRepository
{
  public PartyRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Party?> LoadAsync(PartyId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Party>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Party>> LoadAsync(IEnumerable<PartyId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Party>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Party party, CancellationToken cancellationToken)
  {
    await base.SaveAsync(party, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Party> parties, CancellationToken cancellationToken)
  {
    await base.SaveAsync(parties, cancellationToken);
  }
}
