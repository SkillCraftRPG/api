using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Educations;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class EducationRepository : Repository, IEducationRepository
{
  private readonly GameContext _database;

  public EducationRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Education?> LoadAsync(EducationId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Education>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Education>> LoadAsync(IEnumerable<EducationId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Education>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Education education, CancellationToken cancellationToken)
  {
    await base.SaveAsync(education, cancellationToken);

    await SynchronizeAsync(education, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Education> educations, CancellationToken cancellationToken)
  {
    await base.SaveAsync(educations, cancellationToken);

    foreach (Education education in educations)
    {
      await SynchronizeAsync(education, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Education education, CancellationToken cancellationToken)
  {
    EducationEntity? entity = await _database.Educations.SingleOrDefaultAsync(x => x.StreamId == education.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new EducationEntity(education);
      _database.Educations.Add(entity);
    }
    else
    {
      entity.Update(education);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
