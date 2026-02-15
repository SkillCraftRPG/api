using Logitar.EventSourcing;
using SkillCraft.Api.Core.Educations;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class EducationRepository : Repository, IEducationRepository
{
  public EducationRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Education?> LoadAsync(EducationId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Education>(id.StreamId, cancellationToken);
  }

  public async Task<IReadOnlyCollection<Education>> LoadAsync(IEnumerable<EducationId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Education>(ids.Select(id => id.StreamId), cancellationToken);
  }

  public async Task SaveAsync(Education education, CancellationToken cancellationToken)
  {
    await base.SaveAsync(education, cancellationToken);
  }

  public async Task SaveAsync(IEnumerable<Education> educations, CancellationToken cancellationToken)
  {
    await base.SaveAsync(educations, cancellationToken);
  }
}
