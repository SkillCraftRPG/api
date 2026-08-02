using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using SkillCraft.Api.Core.Castes;
using SkillCraft.Api.Infrastructure.Entities;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class CasteRepository : Logitar.EventSourcing.Repository, ICasteRepository
{
  private readonly GameContext _database;

  public CasteRepository(GameContext database, IEventStore eventStore) : base(eventStore)
  {
    _database = database;
  }

  public async Task<Caste?> LoadAsync(CasteId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Caste>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Caste>> LoadAsync(IEnumerable<CasteId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Caste>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Caste caste, CancellationToken cancellationToken)
  {
    await base.SaveAsync(caste, cancellationToken);

    await SynchronizeAsync(caste, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Caste> castes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(castes, cancellationToken);

    foreach (Caste caste in castes)
    {
      await SynchronizeAsync(caste, cancellationToken);
    }
  }

  private async Task SynchronizeAsync(Caste caste, CancellationToken cancellationToken)
  {
    CasteEntity? entity = await _database.Castes.SingleOrDefaultAsync(x => x.StreamId == caste.Id.Value, cancellationToken);
    if (entity is null)
    {
      entity = new CasteEntity(caste);
      _database.Castes.Add(entity);
    }
    else
    {
      entity.Update(caste);
    }
    await _database.SaveChangesAsync(cancellationToken);
  }
}
