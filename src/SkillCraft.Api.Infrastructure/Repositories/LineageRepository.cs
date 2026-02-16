using Logitar.EventSourcing;
using SkillCraft.Api.Core.Lineages;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class LineageRepository : Repository, ILineageRepository
{
  public LineageRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Lineage?> LoadAsync(LineageId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Lineage>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Lineage>> LoadAsync(IEnumerable<LineageId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Lineage>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Lineage lineage, CancellationToken cancellationToken)
  {
    await base.SaveAsync(lineage, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Lineage> lineages, CancellationToken cancellationToken)
  {
    await base.SaveAsync(lineages, cancellationToken);
  }
}
