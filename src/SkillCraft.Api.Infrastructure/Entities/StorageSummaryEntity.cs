using SkillCraft.Api.Core.Storages.Events;

namespace SkillCraft.Api.Infrastructure.Entities;

internal class StorageSummaryEntity : AggregateEntity
{
  public WorldEntity? World { get; private set; }
  public int WorldId { get; private set; }
  public Guid WorldUid { get; private set; }

  public long AllocatedBytes { get; private set; }
  public long UsedBytes { get; private set; }
  public long RemainingBytes
  {
    get => AllocatedBytes - UsedBytes;
    private set { }
  }

  public List<StorageDetailEntity> Detail { get; private set; } = [];

  public StorageSummaryEntity(WorldEntity world, StorageInitialized @event) : base(@event)
  {
    World = world;
    WorldId = world.WorldId;
    WorldUid = world.Id;

    AllocatedBytes = @event.AllocatedBytes;
  }

  private StorageSummaryEntity() : base()
  {
  }

  public void Store(EntityStored @event)
  {
    Update(@event);

    StorageDetailEntity? detail = Detail.SingleOrDefault(x => x.Key == @event.Key);
    if (detail is null)
    {
      detail = new StorageDetailEntity(this, @event);
      Detail.Add(detail);
    }
    else
    {
      UsedBytes -= detail.Size;
      detail.Update(@event);
    }
    UsedBytes += detail.Size;
  }
}
