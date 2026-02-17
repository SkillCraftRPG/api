using Logitar.EventSourcing;
using SkillCraft.Api.Core.Specializations;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class SpecializationRepository : Repository, ISpecializationRepository
{
  public SpecializationRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Specialization?> LoadAsync(SpecializationId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Specialization>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Specialization>> LoadAsync(IEnumerable<SpecializationId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Specialization>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Specialization specialization, CancellationToken cancellationToken)
  {
    await base.SaveAsync(specialization, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Specialization> specializations, CancellationToken cancellationToken)
  {
    await base.SaveAsync(specializations, cancellationToken);
  }
}
