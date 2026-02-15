using Logitar.EventSourcing;
using SkillCraft.Api.Core.Castes;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CasteRepository : Repository, ICasteRepository
{
  public CasteRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Caste?> LoadAsync(CasteId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Caste>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Caste>> LoadAsync(IEnumerable<CasteId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Caste>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Caste caste, CancellationToken cancellationToken)
  {
    await base.SaveAsync(caste, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Caste> castes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(castes, cancellationToken);
  }
}
