using Logitar.EventSourcing;
using SkillCraft.Api.Core.Storages;

namespace SkillCraft.Api.Infrastructure.Repositories;

internal class StorageRepository : Repository, IStorageRepository
{
  public StorageRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Storage?> LoadAsync(StorageId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Storage>(id.StreamId, cancellationToken);
  }

  public async Task SaveAsync(Storage storage, CancellationToken cancellationToken)
  {
    await base.SaveAsync(storage, cancellationToken);
  }
}
